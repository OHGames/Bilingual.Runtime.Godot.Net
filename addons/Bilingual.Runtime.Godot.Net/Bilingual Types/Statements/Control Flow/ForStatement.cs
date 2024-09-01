using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// A for loop.
    /// </summary>
    public class ForStatement(VariableDeclaration variableDeclaration, Expression loopCondition,
        Expression alterIndex, Block block) : BlockedStatement(block)
    {
        public VariableDeclaration VariableDeclaration { get; set; } = variableDeclaration;
        public Expression LoopCondition { get; set; } = loopCondition;
        public Expression AlterIndex { get; set; } = alterIndex;
    }
}
