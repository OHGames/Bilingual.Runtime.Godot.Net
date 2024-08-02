using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    /// <summary>
    /// Additional data for scripts. Similar to C# attributes.
    /// </summary>
    public class ScriptAttribute : BilingualObject
    {
        public string Name { get; set; }
        public Expression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private ScriptAttribute()
        {
            // used by JSON.
        }

        public ScriptAttribute(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}
