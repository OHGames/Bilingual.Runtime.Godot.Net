using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class DoWhileStatement : Statement
    {
        public Expression Expression { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private DoWhileStatement()
        {
            // used by JSON.
        }

        public DoWhileStatement(Expression expression, Block block)
        {
            Expression = expression;
            Block = block;
        }
    }
}
