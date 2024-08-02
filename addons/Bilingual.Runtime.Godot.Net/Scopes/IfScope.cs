using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// The if statement.
    /// </summary>
    public class IfScope : BlockedScope<IfStatement>
    {
        /// <summary>The loop condition.</summary>
        public Expression IfExpression => Statement.Expression;

        /// <summary>First time running the block and if we need to chose which block to excecute.</summary>
        private bool choseBlockYet;

        public IfScope(Scope? parentScope, VirtualMachine virtualMachine, IfStatement ifStatement) 
            : base(parentScope, virtualMachine, ifStatement)
        {
            Statements = Statement.Block.Statements;
        }

        public override Statement? GetNextStatement()
        {
            if (!choseBlockYet)
            {
                choseBlockYet = true;
                if (!VirtualMachine.EvaluateExpression<bool>(IfExpression))
                {
                    foreach (var elseIf in Statement.ElseIfStatements)
                    {
                        if (VirtualMachine.EvaluateExpression<bool>(elseIf.Expression))
                        {
                            Statements = elseIf.Block.Statements;
                            return base.GetNextStatement();
                        }
                    }

                    // no matching else ifs, check else
                    if (Statement.ElseStatement != null)
                    {
                        Statements = Statement.ElseStatement.Block.Statements;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return base.GetNextStatement();
        }
    }
}
