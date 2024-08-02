using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// An binary or unary expression that uses an operator.
    /// </summary>
    public class OprExpression : Expression
    {
        public Expression? Left { get; set; }
        public Operator Operator { get; set; }
        public Expression? Right { get; set; }

        [Obsolete("Used by JSON only.")]
        private OprExpression()
        {
            // used by JSON.
        }

        /// <param name="left">The left hand side. May be null if unary.</param>
        /// <param name="opr">The operator used in the expression.</param>
        /// <param name="right">The right hand side. May be null if unary.</param>
        public OprExpression(Expression? left, Operator opr, Expression? right)
        {
            Left = left;
            Operator = opr;
            Right = right;
        }
    }
}
