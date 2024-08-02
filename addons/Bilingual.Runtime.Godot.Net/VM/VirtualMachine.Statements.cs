using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.Results;
using Bilingual.Runtime.Godot.Net.Scopes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>The scopes of the running script.</summary>
        public Stack<Scope> Scopes = new Stack<Scope>(256);

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

                default:
                    throw new NotImplementedException();
            }

            return GetNextLine();
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
                var dialogueChunk = ValueToString(EvaluateExpression(expression));
                dialogue += dialogueChunk;
                // TODO: inline functions
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

                default:
                    throw new InvalidOperationException("This blocked scope is not recognized.");
            }

            //CurrentScope = Scopes.Peek();
            return GetNextLine();
        }
    }
}
