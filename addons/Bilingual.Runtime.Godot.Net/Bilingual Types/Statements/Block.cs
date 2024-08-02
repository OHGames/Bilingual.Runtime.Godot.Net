using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A block is a list of statements.
    /// </summary>
    public class Block : BilingualObject
    {
        public List<Statement> Statements { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private Block()
        {
            // used by JSON.
        }

        public Block(List<Statement> statements)
        {
            Statements = statements;
        }
    }
}
