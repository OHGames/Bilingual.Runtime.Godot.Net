using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// An increment or decrement statement.
    /// </summary>
    public class IncrementDecrementStatement(OprExpression expression) : Statement
    {
        public OprExpression Expression { get; set; } = expression;
    }
}
