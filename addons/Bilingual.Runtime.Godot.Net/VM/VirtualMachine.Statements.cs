using Bilingual.Runtime.Godot.Net.BilingualTypes.Containers;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Exceptions;
using Bilingual.Runtime.Godot.Net.Results;
using Bilingual.Runtime.Godot.Net.Scopes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>The scopes of the running script.</summary>
        public Stack<Scope> Scopes = new Stack<Scope>(256);

        /// <summary>While waiting for a choice to be made, hold onto the statement.</summary>
        private ChooseStatement? storedChooseStatement;

        /// <summary>Stores the loaded scripts.
        /// Key is a string with the name of the script, value is the script itself.</summary>
        public readonly Dictionary<string, Script> Scripts = [];

        /// <summary>The current excecuting scope.</summary>
        public Scope? CurrentScope
        {
            get
            {
                _ = Scopes.TryPeek(out Scope? scope);
                return scope;
            }
        }

        public VirtualMachine()
        {
            Scope.GlobalScope.VirtualMachine = this;
            Scopes.Push(Scope.GlobalScope);
        }

        /// <summary> Get the next line of dialogue and run all statements in between.</summary>
        /// <returns>A <see cref="BilingualResult"/>.</returns>
        public BilingualResult GetNextLine()
        {
            if (CurrentScope is null) return new ScriptOver();
            if (storedChooseStatement is not null) return new ErrorResult(ErrorReason.MustSelectChooseOption);

            var statement = CurrentScope.GetNextStatement();

            // Scopes return null when they are over.
            while(statement is null)
            {
                // Get rid of current scope.
                _ = Scopes.Pop();

                // Try and get the next scope.
                if (!Scopes.TryPeek(out Scope? _))
                {
                    // Out of scopes, the statement above was the last statement.
                    return new ScriptOver();
                }

                //CurrentScope = scope;
                statement = CurrentScope.GetNextStatement();
            }

            return RunStatement(statement);
        }

        /// <summary>Run the statements.</summary>
        /// <param name="statement">The statement to run.</param>
        /// <returns>A bilingual result.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private BilingualResult RunStatement(Statement statement)
        {
            switch (statement)
            {
                case ReturnStatement:
                    return new ScriptOver();

                case DialogueStatement dialogueStatement:
                    return RunDialogueStatement(dialogueStatement);

                case ChooseStatement:
                case IfStatement:
                case DoWhileStatement:
                case WhileStatement:
                case ForEachStatement:
                case ForStatement:
                    return RunBlockedScope(statement);

                case VariableAssignment:
                case VariableDeclaration:
                    RunVariableRelatedStatements(statement);
                    break;

                case ContinueStatement:
                case BreakStatement:
                    // handled by Scopes.
                    break;

                case PlusMinusMulDivEqualStatement plusMinusMulDiv:
                    EvaluateExpression(plusMinusMulDiv.Expression);
                    break;

                case IncrementDecrementStatement incrementDecrement:
                    EvaluateExpression(incrementDecrement.Expression);
                    break;

                case FunctionCallStatement functionCallStatement:
                    _ = RunCommandStatement(functionCallStatement.Expression, false);
                    break;

                case RunStatement runStatement:
                    LoadScript(runStatement.Script);
                    break;

                case InjectStatement injectStatement:
                    return RunInjectStatement(injectStatement);

                default:
                    throw new NotImplementedException();
            }

            return GetNextLine();
        }

        /// <summary>Load a script into the VM to run.
        /// Call <see cref="GetNextLine"/> after to get the dialogue.</summary>
        /// <param name="script">The script to inject.</param>
        public void LoadScript(string script)
        {
            if (Scripts.TryGetValue(script, out Script? toLoad))
            {
                // get rid of old stuff
                Scopes.Clear();
                Scopes.Push(Scope.GlobalScope);
                storedChooseStatement = null;

                var newScope = new Scope(Scope.GlobalScope, this);
                newScope.Statements.AddRange(toLoad.Block.Statements);
                Scopes.Push(newScope);
            }
            else
            {
                throw new StatementErrorException($"{script} is has not been loaded.");
            }
        }

        /// <summary>Run a dialogue statement.</summary>
        /// <param name="statement">The dialogue statement.</param>
        /// <returns>A <see cref="DialogueResult"/>.</returns>
        /// <exception cref="InvalidOperationException">When the expression in the dialogue
        /// is not a string <see cref="Literal"/> or an <see cref="InterpolatedString"/>.</exception>
        private BilingualResult RunDialogueStatement(DialogueStatement statement)
        {
            var dialogueText = EvaluateExpression(statement.Dialogue);
            if (dialogueText is string dialogueStr)
            {
                return new DialogueResult(dialogueStr, statement);
            }
            else if (dialogueText is InterpolatedString)
            {
                return RunInterpolatedDialogue(statement);
            }

            throw new InvalidOperationException("Dialogue expression not supported.");
        }

        /// <summary>Run a dialogue statement with an interpolated string.</summary>
        /// <param name="statement">The dialogue statement.</param>
        /// <returns>A <see cref="DialogueResult"/>.</returns>
        public BilingualResult RunInterpolatedDialogue(DialogueStatement statement)
        {
            var interpolated = (InterpolatedString)statement.Dialogue;
            var dialogue = "";

            for (int i = 0; i < interpolated.Expressions.Count; i++)
            {
                var expression = interpolated.Expressions[i];
                string dialogueChunk;

                if (expression is FunctionCallExpression function)
                {
                    dialogueChunk = RunCommandStatement(function, true) ?? "null";
                }
                else
                {
                    dialogueChunk = ValueToString(EvaluateExpression(expression));
                }

                dialogue += dialogueChunk;
            }

            return new DialogueResult(dialogue, statement);
        }

        /// <summary>Convert an object to a string.</summary>
        /// <param name="value">The value of the expression.</param>
        /// <returns>A string representation of the object.</returns>
        private string ValueToString(object value)
        {
            if (value is null) return "null";

            if (value is string str)
            {
                // catch a string before the next if statement because
                // strings are also enumerable and we dont want to
                // convert a string to an array of chars.
                return str;
            }
            else if (value is IEnumerable objects)
            {
                var listStr = "[";
                foreach (var item in objects)
                {
                    listStr += item.ToString() + ", ";
                }
                // cut off the last item's comma and space.
                listStr = listStr[..^2] + "]";
                return listStr;
            }
            else
            {
                return value.ToString() ?? "null";
            }
        }

        /// <summary>Run a variable related statement.</summary>
        /// <param name="statement">The statement.</param>
        /// <exception cref="InvalidOperationException">If the statement is not handled.</exception>
        private void RunVariableRelatedStatements(Statement statement)
        {
            if (CurrentScope is null) 
                throw new InvalidOperationException("Scope cannot be null here.");

            if (statement is VariableDeclaration declaration)
            {
                var scope = declaration.Global ? Scope.GlobalScope : CurrentScope;
                var value = EvaluateExpression(declaration.Expression);
                scope.AddNewVariable(declaration.Name, value);
            }
            else if (statement is VariableAssignment assignment)
            {
                var value = EvaluateExpression(assignment.Expression);
                CurrentScope.UpdateVariableValue(assignment.Variable, value);
            }
            else
            {
                throw new InvalidOperationException("Variable related statement not handled.");
            }
        }

        /// <summary>Select an option for a choose statement.</summary>
        /// <param name="option">The option to select.</param>
        /// <exception cref="StatementErrorException">When there is no choose statement stored.</exception>
        public void SelectOption(string option)
        {
            if (storedChooseStatement is null) throw new StatementErrorException("No stored choose statement.");
            if (CurrentScope is null) throw new InvalidOperationException("There are no more scopes.");

            var chooseScope = (ChooseScope)CurrentScope;
            chooseScope.SelectOption(option);

            storedChooseStatement = null;
        }

        /// <summary>Run a blocked scope.</summary>
        /// <param name="statement">The blocked scope.</param>
        /// <returns>A bilingual result.</returns>
        /// <exception cref="InvalidOperationException">When the scoped statement is not recognized.</exception>
        private BilingualResult RunBlockedScope(Statement statement)
        {
            switch (statement)
            {
                case IfStatement ifStatement:
                    var ifScope = new IfScope(CurrentScope, this, ifStatement);
                    Scopes.Push(ifScope);
                    break;

                case DoWhileStatement doWhile:
                    var doWhileScope = new DoWhileScope(CurrentScope, this, doWhile);
                    Scopes.Push(doWhileScope);
                    break;

                case WhileStatement whileStatement:
                    var whileScope = new WhileScope(CurrentScope, this, whileStatement);
                    Scopes.Push(whileScope);
                    break;

                case ForEachStatement forEach:
                    var forEachScope = new ForEachScope(CurrentScope, this, forEach);
                    Scopes.Push(forEachScope);
                    break;

                case ForStatement forStatement:
                    var forScope = new ForScope(CurrentScope, this, forStatement);
                    Scopes.Push(forScope);
                    break;

                case ChooseStatement chooseStatement:
                    var chooseScope = new ChooseScope(CurrentScope, this, chooseStatement);
                    Scopes.Push(chooseScope);
                    storedChooseStatement = chooseStatement;
                    return new ChooseOptionsResult(chooseScope.GetOptions());

                default:
                    throw new InvalidOperationException("This blocked scope is not recognized.");
            }

            //CurrentScope = Scopes.Peek();
            return GetNextLine();
        }

        /// <summary>
        /// Run a command (a function provided to the vm).
        /// </summary>
        /// <returns>A string or null.</returns>
        /// <param name="funcExpression">The function to call.</param>
        /// <param name="inline">If the command is being run inline.</param>
        public string? RunCommandStatement(FunctionCallExpression funcExpression, bool inline)
        {
            var name = funcExpression.Name; // todo? accessors
            List<object> parameters = [];

            foreach (var param in funcExpression.Params.Expressions)
            {
                parameters.Add(EvaluateExpression(param));
            }

            if (CommandStore.Commands.TryGetValue(name, out SynchronousFunctionDelegate? sync))
            {
                sync(parameters);
                return null;
            }
            else if (CommandStore.AsyncCommands.TryGetValue(name, out AsyncFunctionDelegate? async))
            {
                if (funcExpression.Await)
                {
                    // https://stackoverflow.com/a/55516918
                    // this acts like 'await async(..)'
                    var task = Task.Run(() => async(parameters));
                    task.Wait();
                }
                else
                {
                    // run asyncronously
                    async(parameters);
                }
                return null;
            }
            else if (inline && CommandStore.InlineCommands.TryGetValue(name, 
                out InlineSyncronousFuncDelegate? inlineFunc))
            {
                return inlineFunc(parameters);
            }
            else
            {
                throw new InvalidOperationException($"Cannot find {name} command.");
            }
        }

        /// <summary>Run the inject statement.</summary>
        /// <param name="inject">The script to inject.</param>
        /// <returns>The next line of dialogue.</returns>
        private BilingualResult RunInjectStatement(InjectStatement inject)
        {
            var scope = CurrentScope ?? throw new InvalidOperationException("CurrentScope should not be null");
            
            if (Scripts.TryGetValue(inject.Script, out Script? script))
            {
                scope.InjectStatements(script.Block);
            }
            else
            {
                throw new StatementErrorException($"{inject.Script} is not loaded.");
            }

            return GetNextLine();
        }
    }
}
