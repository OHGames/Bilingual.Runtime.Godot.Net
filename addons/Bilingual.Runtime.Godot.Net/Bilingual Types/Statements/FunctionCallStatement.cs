using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// When a function call is made as a statement. See <see cref="FunctionCallExpression"/>.
    /// </summary>
    public class FunctionCallStatement : Statement
    {
        public FunctionCallExpression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private FunctionCallStatement()
        {
            // used by JSON.
        }

        /// <param name="expression">The function call.</param>
        public FunctionCallStatement(FunctionCallExpression expression)
        {
            Expression = expression;
        }
    }
}
