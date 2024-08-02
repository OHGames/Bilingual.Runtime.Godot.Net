using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Containers
{
    /// <summary>
    /// A Script contains a list of statements to run.
    /// </summary>
    public class Script : BilingualObject
    {
        public string Name { get; set; }
        public Block Block { get; set; }
        public List<ScriptAttribute> Attributes { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private Script()
        {
            // used by JSON.
        }

        public Script(string name, Block block, List<ScriptAttribute> attributes)
        {
            Name = name;
            Block = block;
            Attributes = attributes;
        }
    }
}
