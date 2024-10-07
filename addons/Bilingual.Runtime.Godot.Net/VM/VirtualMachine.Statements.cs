using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Exceptions;
using Bilingual.Runtime.Godot.Net.Localization;
using Bilingual.Runtime.Godot.Net.Nodes;
using Bilingual.Runtime.Godot.Net.Results;
using Bilingual.Runtime.Godot.Net.Scopes;
using CLDRPlurals;
using Godot;
using Humanizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Script = Bilingual.Runtime.Godot.Net.BilingualTypes.Containers.Script;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>The scopes of the running script.</summary>
        public Stack<Scope> Scopes = new Stack<Scope>(256);

        /// <summary>While waiting for a choice to be made, hold onto the statement.</summary>
        private ChooseStatement? storedChooseStatement;

        /// <summary>While waiting for an inline Wait, store the rest of the statement.</summary>
        private DialogueStatement? storedDialogueStatement;

        /// <summary>Stores the loaded scripts.
        /// Key is a string with the name of the script, value is the script itself.</summary>
        public readonly Dictionary<string, Script> Scripts = [];

        /// <summary>If the VM should wait instead of having the user
        /// handle wait statements. If this is true (which it is by default), 
        /// the timer will be created and no dialogue can be called until wait 
        /// is over. Turn this off to control the specific timing
        /// for wait statements. The VM will use the <see cref="SceneTree.CreateTimer(double, bool, bool, bool)"/> default
        /// settings for determinig the time to wait.</summary>
        public bool UseVmToWait { get; set; } = true;

        /// <summary>When the dialogue becomes paused.
        /// Helpful when <see cref="UseVmToWait"/> is true.</summary>
        /// <param name="result">The dialogue result. Can be inline or not.</param>
        public delegate void DialoguePaused(BilingualResult result);

        /// <summary>Emits the dialogue runner signal <see cref="DialogueRunner.DialoguePaused"/>.</summary>
        internal DialoguePaused PausedCallback = delegate { };

        /// <summary>Emits the dialogue runner signal <see cref="DialogueRunner.DialogueResumed"/>.</summary>
        internal Action ResumedCallback = delegate { };

        /// <summary>Emits <see cref="DialogueRunner.ScriptStartedRunning"/>.
        /// This runs on 'run' commands and 'inject' commands.</summary>
        internal Action<Dictionary<string, object>> ScriptStartedRunningCallback = delegate { };

        /// <summary>If dialogue is paused.</summary>
        private bool paused;

        /// <summary>The scene tree.</summary>
        public SceneTree? tree;

        /// <summary>The current excecuting scope.</summary>
        public Scope? CurrentScope
        {
            get
            {
                _ = Scopes.TryPeek(out Scope? scope);
                return scope;
            }
        }

        /// <summary>If translation should occur.</summary>
        private static bool ShouldTranslate => BilingualTranslationService.TranslationSettings.ShouldTranslate;

        /// <summary>The full name of the current script.</summary>
        private string currentScriptName = "";

        public VirtualMachine()
        {
            Scope.GlobalScope.VirtualMachine = this;
            Scopes.Push(Scope.GlobalScope);
        }

        /// <summary> Get the next line of dialogue and run all statements in between.</summary>
        /// <returns>A <see cref="BilingualResult"/>.</returns>
        public async Task<BilingualResult> GetNextLine()
        {
            if (UseVmToWait && paused)
            {
                return new WarningResult(WarningReason.ScriptPaused);
            }

            if (CurrentScope is null) return new ScriptOver();
            if (storedChooseStatement is not null) return new WarningResult(WarningReason.MustSelectChooseOption);
            if (storedDialogueStatement is not null)
            {
                var copyStored = storedDialogueStatement.Copy();
                // set null before so we dont keep calling the stored when we dont need it
                // if this block needs to run again, RunInterpolatedDialogue will set it again
                storedDialogueStatement = null;
                return await RunInterpolatedDialogue(copyStored, true);
            }

            var statement = CurrentScope.GetNextStatement();

            // Scopes return null when they are over.
            while (statement is null)
            {
                // Get rid of current scope.
                _ = Scopes.Pop();

                // Try and get the next scope.
                if (!Scopes.TryPeek(out Scope? _))
                {
                    // Out of scopes, the statement above was the last statement.
                    return new ScriptOver();
                }

                statement = CurrentScope.GetNextStatement();
            }

            return await RunStatement(statement);
        }

        /// <summary>Run the statements.</summary>
        /// <param name="statement">The statement to run.</param>
        /// <returns>A bilingual result.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<BilingualResult> RunStatement(Statement statement)
        {
            switch (statement)
            {
                case ReturnStatement:
                    return new ScriptOver();

                case DialogueStatement dialogueStatement:
                    return await RunDialogueStatement(dialogueStatement);

                case ChooseStatement:
                case IfStatement:
                case DoWhileStatement:
                case WhileStatement:
                case ForEachStatement:
                case ForStatement:
                    return await RunBlockedScope(statement);

                case VariableAssignment:
                case VariableDeclaration:
                    RunVariableRelatedStatements(statement);
                    break;

                case ContinueStatement:
                    var loop = GoToParentLoopScope();
                    if (loop is null) break;

                    loop.Continue();
                    break;

                case BreakStatement:
                    var loopToBreak = GoToParentLoopScope();
                    if (loopToBreak is null) break;

                    loopToBreak.Break();
                    break;

                case PlusMinusMulDivEqualStatement plusMinusMulDiv:
                    EvaluateExpression(plusMinusMulDiv.Expression);
                    break;

                case IncrementDecrementStatement incrementDecrement:
                    EvaluateExpression(incrementDecrement.Expression);
                    break;

                case FunctionCallStatement functionCallStatement:
                    var expr = functionCallStatement.Expression;
                    if (CheckBuiltInCommands(expr.Name))
                    {
                        return RunBuiltInCommand(expr.Name, expr);
                    }
                    else
                    {
                        var result = RunCommandStatement(functionCallStatement.Expression, false);
                        if (result != null)
                            await result;
                    }
                    break;

                case RunStatement runStatement:
                    LoadScript(runStatement.Script);
                    break;

                case InjectStatement injectStatement:
                    return await RunInjectStatement(injectStatement);

                case EndInjectStatement endInject:
                    currentScriptName = endInject.PreviousScriptName;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return await GetNextLine();
        }

        /// <summary>Move to the parent loop scope.</summary>
        private IBreakableScope? GoToParentLoopScope()
        {
            if (CurrentScope is null) throw new InvalidOperationException("Current Scope is null");
            var loop = CurrentScope.loopParent;
            if (loop is null) return null;
            while (Scopes.Peek() != loop)
            {
                _ = Scopes.Pop();
            }
            return loop;
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
                PushScope(newScope);
                //Scopes.Push(newScope);
                currentScriptName = script;

                var dict = GetScriptAttributes();
                ScriptStartedRunningCallback(dict);
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
        private async Task<DialogueResult> RunDialogueStatement(DialogueStatement statement)
        {
            var dialogue = statement.Dialogue;
            if (!ShouldTranslate)
            {
                // If translated, the stuff is already escaped.
                // If not, we need to escape it now.
                dialogue = BilingualTranslationService.EscapeExpression(dialogue, statement.LineId ?? 0);
            }

            var dialogueText = EvaluateExpression(dialogue);
            if (dialogueText is string dialogueStr)
            {
                if (ShouldTranslate)
                {
                    var id = statement.LineId ?? throw new Exception("Line id missing");
                    var translation = BilingualTranslationService.Translate(id);
                    var lit = (Literal)translation;
                    return new DialogueResult((string)lit, statement, false);
                }

                return new DialogueResult(dialogueStr, statement, false);
            }
            else if (dialogueText is InterpolatedString)
            {
                return await RunInterpolatedDialogue(statement, false);
            }

            throw new InvalidOperationException("Dialogue expression not supported.");
        }

        /// <summary>Run a dialogue statement with an interpolated string.</summary>
        /// <param name="statement">The dialogue statement.</param>
        /// <param name="moreLeft">If there is more of the line left.</param>
        /// <param name="getFullString">If the call is actually getting the full text of the statement.</param>
        /// <returns>A <see cref="DialogueResult"/>.</returns>
        private async Task<DialogueResult> RunInterpolatedDialogue(DialogueStatement statement, bool prevPaused,
            bool getFullString = false)
        {
            var interpolated = (InterpolatedString)statement.Dialogue;
            var dialogue = "";

            if (ShouldTranslate)
            {
                var id = statement.LineId ?? throw new Exception("Line id missing");
                var translated = BilingualTranslationService.Translate(id);

                if (translated is Literal)
                {
                    // If the interpolated stuff was removed a single literal is returned.
                    // So just make it into an interpolated string.
                    interpolated = new InterpolatedString([translated]);
                }
                else
                {
                    interpolated = (InterpolatedString)translated;
                }
            }

            for (int i = 0; i < interpolated.Expressions.Count; i++)
            {
                var expression = interpolated.Expressions[i];
                string dialogueChunk = "";

                if (expression is FunctionCallExpression function)
                {
                    if (CheckBuiltInCommands(function.Name))
                    {
                        // Built in commands should not return anything right now (only 1 so far),
                        // so only run when not getting a full string.
                        if (!getFullString)
                        {
                            var seconds = GetWaitTime(function);
                            // Get the interpolated dialogue with functions removed.
                            var fullString = (await RunInterpolatedDialogue(statement,
                                prevPaused, true)).Dialogue;
                            var rest = new InterpolatedString(interpolated.Expressions[(i + 1)..]);

                            // copy the statement so the original is not altered.
                            storedDialogueStatement = statement.CopyWithNewDialogue(rest);

                            var result = new ScriptPausedInlineResult(dialogue, statement, seconds,
                                prevPaused ? "" : fullString, prevPaused);

                            if (UseVmToWait)
                            {
                                paused = true;
                                StartWait(seconds);
                                PausedCallback(result);
                            }

                            return result;
                        }
                    }
                    else
                    {
                        dialogueChunk = await RunCommandStatement(function, true) ?? "null";
                    }
                }
                else if (expression is LocalizedQuanity quanity)
                {
                    var value = EvaluateExpression<double>(quanity.Value);
                    PluralCase pluralType = PluralCase.Other;
                    var locale = BilingualTranslationService.TranslateIntoBasic;
                    if (quanity.Cardinal)
                    {
                        pluralType = NumberPlurals.GetCardinalPluralCase(locale, value);
                    }
                    else
                    {
                        pluralType = NumberPlurals.GetOrdinalPluralCase(locale, value);
                    }

                    var localizedString = quanity.Plurals[pluralType];

                    // If here is a #, replace it with the expression's value.
                    var hashTagRegex = BilingualTranslationService.MatchHashTag();
                    var matches = hashTagRegex.Matches(localizedString);
                    if (matches.Count == 0) dialogueChunk = localizedString;
                    else
                    {
                        dialogueChunk = hashTagRegex.Replace(localizedString, (match) =>
                            ReplaceHashtag(match, value, quanity.Cardinal)
                        );
                    }

                    // Escape. The regex made sure that the escaped # are not repaced.
                    dialogueChunk = BilingualTranslationService.EscapeBackSlashes(dialogueChunk, statement.LineId ?? 0);
                }
                else
                {
                    dialogueChunk = ValueToString(EvaluateExpression(expression));
                }

                dialogue += dialogueChunk;
            }

            return new DialogueResult(dialogue, statement, prevPaused);
        }

        /// <summary>Replace the hashtag in a localized quanity with a string.</summary>
        /// <param name="match">The regex match.</param>
        /// <param name="value">The value</param>
        /// <returns>The string.</returns>
        private static string ReplaceHashtag(Match match, double value, bool cardinal)
        {
            if (match.Length == 1)
                return value.ToString();
            else
            {
                // Double hashtag should replace with words
                var culture = new CultureInfo(BilingualTranslationService.TranslateInto);
                var gender = GrammaticalGender.Masculine;
                var form = WordForm.Normal;
                bool capitalize = false;

                // we have a gender and/or abbreviation if true
                if (match.Length != 2)
                {
                    var markedGender = match.Value[2];
                    gender = markedGender switch
                    {
                        'n' => GrammaticalGender.Neuter,
                        'N' => GrammaticalGender.Neuter,
                        'f' => GrammaticalGender.Feminine,
                        'F' => GrammaticalGender.Feminine,
                        _ => GrammaticalGender.Masculine
                    };
                    capitalize = markedGender == 'N' || markedGender == 'F' || markedGender == 'M';

                    // abbreviation
                    if (match.Length == 4 && match.Value[3] == 'a')
                    {
                        form = WordForm.Abbreviation;
                    }
                }

                // Humanizer only supports whole numbers for words.
                if (IsWholeNumber(value))
                {
                    long longValue = (long)value;
                    int intValue = (int)value;
                    var words = cardinal
                        ? longValue.ToWords(form, gender, culture)
                        : intValue.ToOrdinalWords(gender, form, culture);

                    if (capitalize)
                        return words.Transform(culture, To.SentenceCase);
                    else
                        return words;
                }

                return value.ToString();
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
        public void SelectOption(int index)
        {
            if (storedChooseStatement is null) throw new StatementErrorException("No stored choose statement.");
            if (CurrentScope is null) throw new InvalidOperationException("There are no more scopes.");

            var chooseScope = (ChooseScope)CurrentScope;
            chooseScope.SelectOption(index);

            storedChooseStatement = null;
        }

        /// <summary>Run a blocked scope.</summary>
        /// <param name="statement">The blocked scope.</param>
        /// <returns>A bilingual result.</returns>
        /// <exception cref="InvalidOperationException">When the scoped statement is not recognized.</exception>
        private async Task<BilingualResult> RunBlockedScope(Statement statement)
        {
            switch (statement)
            {
                case IfStatement ifStatement:
                    var ifScope = new IfScope(CurrentScope, this, ifStatement);
                    PushScope(ifScope);
                    //Scopes.Push(ifScope);
                    break;

                case DoWhileStatement doWhile:
                    var doWhileScope = new DoWhileScope(CurrentScope, this, doWhile);
                    PushScope(doWhileScope);
                    break;

                case WhileStatement whileStatement:
                    var whileScope = new WhileScope(CurrentScope, this, whileStatement);
                    PushScope(whileScope);
                    break;

                case ForEachStatement forEach:
                    var forEachScope = new ForEachScope(CurrentScope, this, forEach);
                    PushScope(forEachScope);
                    break;

                case ForStatement forStatement:
                    var forScope = new ForScope(CurrentScope, this, forStatement);
                    PushScope(forScope);
                    break;

                case ChooseStatement chooseStatement:
                    var chooseScope = new ChooseScope(CurrentScope, this, chooseStatement);
                    PushScope(chooseScope);
                    storedChooseStatement = chooseStatement;
                    return new ChooseOptionsResult(chooseScope.GetOptions());

                default:
                    throw new InvalidOperationException("This blocked scope is not recognized.");
            }

            return await GetNextLine();
        }

        /// <summary>
        /// Run a command (a function provided to the vm).
        /// </summary>
        /// <returns>A string or null.</returns>
        /// <param name="funcExpression">The function to call.</param>
        /// <param name="inline">If the command is being run inline.</param>
        public async Task<string>? RunCommandStatement(FunctionCallExpression funcExpression, bool inline)
        {
            var name = funcExpression.Name;
            List<object> parameters = [];

            foreach (var param in funcExpression.Params.Expressions)
            {
                parameters.Add(EvaluateExpression(param));
            }

            if (CommandStore.Commands.TryGetValue(name, out SynchronousFunctionDelegate? sync))
            {
                sync(parameters);
                return null!;
            }
            else if (CommandStore.AsyncCommands.TryGetValue(name, out AsyncFunctionDelegate? async))
            {
                if (funcExpression.Await)
                {
                    await async(parameters);
                }
                else
                {
                    // run asyncronously
                    _ = async(parameters);
                }
                return null!;
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
        private async Task<BilingualResult> RunInjectStatement(InjectStatement inject)
        {
            var scope = CurrentScope ?? throw new InvalidOperationException("CurrentScope should not be null");

            if (Scripts.TryGetValue(inject.Script, out Script? script))
            {
                scope.InjectStatements(script.Block);
                scope.Statements.Add(new EndInjectStatement(currentScriptName));
            }
            else
            {
                throw new StatementErrorException($"{inject.Script} is not loaded.");
            }

            currentScriptName = inject.Script;
            ScriptStartedRunningCallback(GetScriptAttributes());

            return await GetNextLine();
        }

        /// <summary>Get the attributes of the current running script.</summary>
        /// <returns>A dictionary of attributes. The key is the name and the value 
        /// is the attribute value.</returns>
        public Dictionary<string, object> GetScriptAttributes()
        {
            var script = Scripts[currentScriptName];
            Dictionary<string, object> attributes = [];

            foreach (var attr in script.Attributes)
            {
                var name = attr.Name;
                var expr = EvaluateExpression(attr.Value);
                attributes.Add(name, expr);
            }

            return attributes;
        }

        /// <summary>Push a new scope to the stack.</summary>
        /// <param name="newScope"></param>
        private void PushScope(Scope newScope)
        {
            IBreakableScope? parentLoop = null;

            // Set the parent loop.
            if (newScope is not IBreakableScope)
            {
                // foreach on a stack also moves in LIFO pattern.
                foreach (var scope in Scopes)
                {
                    if (scope is IBreakableScope breakable)
                    {
                        parentLoop = breakable;
                        break;
                    }
                }
            }

            newScope.loopParent = parentLoop;
            Scopes.Push(newScope);
        }
    }
}
