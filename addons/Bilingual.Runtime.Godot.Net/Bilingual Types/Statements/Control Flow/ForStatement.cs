using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// A for loop.
    /// </summary>
    public class ForStatement : Statement
    {
        public VariableDeclaration VariableDeclaration { get; set; }
        public Expression LoopCondition { get; set; }
        public Expression AlterIndex { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private ForStatement()
        {
            // used by JSON.
        }

        public ForStatement(VariableDeclaration variableDeclaration, Expression loopCondition,
            Expression alterIndex, Block block)
        {
            VariableDeclaration = variableDeclaration;
            LoopCondition = loopCondition;
            AlterIndex = alterIndex;
            Block = block;
        }
    }
}
