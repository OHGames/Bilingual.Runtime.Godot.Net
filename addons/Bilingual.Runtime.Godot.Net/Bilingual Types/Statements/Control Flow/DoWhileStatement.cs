using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class DoWhileStatement(Expression expression, Block block) : Statement
    {
        public Expression Expression { get; set; } = expression;
        public Block Block { get; set; } = block;
    }
}
