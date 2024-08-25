namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// An binary or unary expression that uses an operator.
    /// </summary>
    /// <param name="left">The left hand side. May be null if unary.</param>
    /// <param name="opr">The operator used in the expression.</param>
    /// <param name="right">The right hand side. May be null if unary.</param>
    public class OprExpression(Expression? left, Operator opr, Expression? right) : Expression
    {
        public Expression? Left { get; set; } = left;
        public Operator Operator { get; set; } = opr;
        public Expression? Right { get; set; } = right;
    }
}
