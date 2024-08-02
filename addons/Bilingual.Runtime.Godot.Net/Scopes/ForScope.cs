using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    public class ForScope : BlockedScope<ForStatement>
    {
        public VariableDeclaration VariableDeclaration => Statement.VariableDeclaration;
        public Expression LoopCondition => Statement.LoopCondition;
        public Expression AlterIndex => Statement.AlterIndex;

        private bool endOfLoop = true;
        private bool firstLoop = true;

        public ForScope(Scope? parentScope, VirtualMachine virtualMachine, ForStatement forStatement) 
            : base(parentScope, virtualMachine, forStatement)
        {
            Statements = forStatement.Block.Statements;
        }

        public override Statement? GetNextStatement()
        {
            if (firstLoop)
            {
                AddNewVariable(VariableDeclaration.Name, VirtualMachine.EvaluateExpression(VariableDeclaration.Expression));
                firstLoop = false;
            }

            // start of loop
            if (currentStatement == 0)
            {
                // check the loop condition
                if (!VirtualMachine.EvaluateExpression<bool>(LoopCondition)) return null;
            }

            var line = base.GetNextStatement();
            endOfLoop = line is null;
            if (line is BreakStatement) return null;
            if (line is ContinueStatement) endOfLoop = true;

            if (endOfLoop)
            {
                _ = VirtualMachine.EvaluateExpression<double>(AlterIndex);
                currentStatement = 0;
                return GetNextStatement();
            }

            return line;
        }
    }
}
