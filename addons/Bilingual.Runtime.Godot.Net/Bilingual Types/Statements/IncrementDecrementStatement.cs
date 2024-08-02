using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// An increment or decrement statement.
    /// </summary>
    public class IncrementDecrementStatement : Statement
    {
        public OprExpression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private IncrementDecrementStatement()
        {
            // used by JSON.
        }

        public IncrementDecrementStatement(OprExpression expression)
        {
            Expression = expression;
        }
    }
}
