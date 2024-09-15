using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A while loop
    /// </summary>
    public class WhileScope : BlockedScope<WhileStatement>, IBreakableScope
    {
        /// <summary>The expression to check to keep looping.</summary>
        public Expression LoopCondition => Statement.Expression;

        /// <summary>If the first loop has checked the condition to start or skip the loop.</summary>
        private bool hasCheckedOnFirstLoop = true;

        private bool broken = false;

        public WhileScope(Scope? parentScope, VirtualMachine vm, WhileStatement whileStatement) 
            : base(parentScope, vm, whileStatement)
        {
            Statements = whileStatement.Block.Statements;
            loopParent = this;
        }

        public override Statement? GetNextStatement()
        {
            if (broken) return null;

            var line = base.GetNextStatement();

            // check, then run loop
            if (hasCheckedOnFirstLoop || line is null)
            {
                if (VirtualMachine.EvaluateExpression<bool>(LoopCondition))
                {
                    // continue looping
                    if (line is null)
                    {
                        currentStatement = 0;
                        line = base.GetNextStatement();
                    }

                    hasCheckedOnFirstLoop = false;
                }
                else
                {
                    // quit looping
                    return null;
                }
            }

            // we are finished or need to continue
            if (line is BreakStatement) return null;
            if (line is ContinueStatement)
            {
                hasCheckedOnFirstLoop = true;
                return GetNextStatement();
            }

            return line;
        }

        public void Break()
        {
            broken = true;
        }

        public void Continue()
        {
            currentStatement = 0;
            hasCheckedOnFirstLoop = true;
        }
    }
}
