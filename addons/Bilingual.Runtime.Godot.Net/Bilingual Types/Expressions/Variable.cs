namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A variable.
    /// </summary>
    public class Variable(string name) : Expression
    {
        public string Name { get; set; } = name;
    }
}
