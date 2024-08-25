using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A statement in which a variable uses compound assignemnt <c>(+= -= *= /=)</c>
    /// </summary>
    public class PlusMinusMulDivEqualStatement(OprExpression expression) : Statement
    {
        public OprExpression Expression { get; set; } = expression;
    }
}
