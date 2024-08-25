using Bilingual.Runtime.Godot.Net.BilingualTypes.Containers;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    /// <summary>
    /// A representation of a .bi file.
    /// </summary>
    public class BilingualFile(List<ScriptContainer> containers) : BilingualObject
    {
        public List<ScriptContainer> ScriptContainers { get; set; } = containers;
    }
}
