using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Containers
{
    /// <summary>
    /// A Script contains a list of statements to run.
    /// </summary>
    public class Script(string name, Block block, List<ScriptAttribute> attributes) : BilingualObject
    {
        public string Name { get; set; } = name;
        public Block Block { get; set; } = block;
        public List<ScriptAttribute> Attributes { get; set; } = attributes;
    }
}
