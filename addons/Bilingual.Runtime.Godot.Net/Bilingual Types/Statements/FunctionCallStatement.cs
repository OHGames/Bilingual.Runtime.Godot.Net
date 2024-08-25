using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// When a function call is made as a statement. See <see cref="FunctionCallExpression"/>.
    /// </summary>
    /// <param name="expression">The function call.</param>
    public class FunctionCallStatement(FunctionCallExpression expression) : Statement
    {
        public FunctionCallExpression Expression { get; set; } = expression;
    }
}
