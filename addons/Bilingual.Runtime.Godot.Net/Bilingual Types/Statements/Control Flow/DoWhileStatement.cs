using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class DoWhileStatement(Expression expression, Block block) : BlockedStatement(block)
    {
        public Expression Expression { get; set; } = expression;
    }
}
