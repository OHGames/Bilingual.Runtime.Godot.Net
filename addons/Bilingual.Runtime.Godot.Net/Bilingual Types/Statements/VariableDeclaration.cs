using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// Creating a new variable.
    /// </summary>
    public class VariableDeclaration(string name, bool global, Expression expression) : Statement
    {
        public string Name { get; set; } = name;
        public bool Global { get; set; } = global;
        public Expression Expression { get; set; } = expression;
    }
}
