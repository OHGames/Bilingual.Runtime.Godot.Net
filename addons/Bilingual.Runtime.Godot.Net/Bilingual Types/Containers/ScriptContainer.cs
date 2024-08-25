
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Containers
{
    /// <summary>
    /// A script container is like a C# namespace, it holds scripts.
    /// </summary>
    public class ScriptContainer(string name, List<Script> scripts) : BilingualObject
    {
        public string Name { get; set; } = name;
        public List<Script> Scripts { get; set; } = scripts;
    }
}
