using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A do-while loop.
    /// </summary>
    public class DoWhileScope : BlockedScope<DoWhileStatement>
    {
        /// <summary>The expression to check to keep looping.</summary>
        public Expression LoopCondition => Statement.Expression;

        /// <summary>If all the statements have been run for this one loop.</summary>
        private bool reachedEndOfLoop;

        public DoWhileScope(Scope? parentScope, VirtualMachine vm, DoWhileStatement doWhileStatement) 
            : base(parentScope, vm, doWhileStatement)
        {
            Statements = doWhileStatement.Block.Statements;
        }

        public override Statement? GetNextStatement()
        {
            // run loop first, then check.
            var line = base.GetNextStatement();
            reachedEndOfLoop = line is null;

            if (line is BreakStatement) return null;
            if (line is ContinueStatement)
            {
                reachedEndOfLoop = true;
            }

            if (reachedEndOfLoop)
            {
                if (VirtualMachine.EvaluateExpression<bool>(LoopCondition))
                {
                    // continue looping
                    currentStatement = 0;
                    return base.GetNextStatement();
                }
                else
                {
                    // quit looping
                    return null;
                }
            }

            return line;
        }
    }
}
