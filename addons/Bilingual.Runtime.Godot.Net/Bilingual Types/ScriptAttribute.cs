using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    /// <summary>
    /// Additional data for scripts. Similar to C# attributes.
    /// </summary>
    public class ScriptAttribute(string name, Expression value) : BilingualObject
    {
        public string Name { get; set; } = name;
        public Expression Value { get; set; } = value;
    }
}
