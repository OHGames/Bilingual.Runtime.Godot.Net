
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Containers
{
    /// <summary>
    /// A script container is like a C# namespace, it holds scripts.
    /// </summary>
    public class ScriptContainer : BilingualObject
    {
        public string Name { get; set; }
        public List<Script> Scripts { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private ScriptContainer()
        {
            // used by JSON.
        }

        public ScriptContainer(string name, List<Script> scripts)
        {
            Name = name;
            Scripts = scripts;
        }
    }
}
