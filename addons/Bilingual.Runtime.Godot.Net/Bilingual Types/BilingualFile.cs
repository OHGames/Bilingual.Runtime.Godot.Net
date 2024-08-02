using Bilingual.Runtime.Godot.Net.BilingualTypes.Containers;
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    /// <summary>
    /// A representation of a .bi file.
    /// </summary>
    public class BilingualFile : BilingualObject
    {
        public List<ScriptContainer> ScriptContainers { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private BilingualFile()
        {
            // used by JSON.
        }

        public BilingualFile(List<ScriptContainer> containers)
        {
            ScriptContainers = containers;
        }
    }
}
