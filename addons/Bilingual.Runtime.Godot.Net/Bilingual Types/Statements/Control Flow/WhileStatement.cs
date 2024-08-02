using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class WhileStatement : Statement
    {
        public Expression Expression { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private WhileStatement()
        {
            // used by JSON.
        }

        public WhileStatement(Expression expression, Block block)
        {
            Expression = expression;
            Block = block;
        }
    }
}
