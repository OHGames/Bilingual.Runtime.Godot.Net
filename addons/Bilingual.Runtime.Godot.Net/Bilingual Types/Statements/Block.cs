using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A block is a list of statements.
    /// </summary>
    public class Block(List<Statement> statements) : BilingualObject
    {
        public List<Statement> Statements { get; set; } = statements;

        // Shorthand to get statements.
        public static implicit operator List<Statement>(Block b) => b.Statements;
    }
}
