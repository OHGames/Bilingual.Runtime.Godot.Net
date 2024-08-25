using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// Changing a variable's value.
    /// </summary>
    public class VariableAssignment(Variable name, Expression expression) : Statement
    {
        public Variable Variable { get; set; } = name;
        public Expression Expression { get; set; } = expression;
    }
}
