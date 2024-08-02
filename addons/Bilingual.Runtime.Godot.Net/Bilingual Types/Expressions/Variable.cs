using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A variable.
    /// </summary>
    public class Variable : Expression
    {
        public string Name { get; set; }
        public List<Accessor> Accessors { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private Variable()
        {
            // used by JSON.
        }

        public Variable(string name, List<Accessor> accessors)
        {
            Name = name;
            Accessors = accessors;
        }
    }
}
