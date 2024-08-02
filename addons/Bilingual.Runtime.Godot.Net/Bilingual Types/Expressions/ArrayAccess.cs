using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// When a function call or a member gets from an array.
    /// </summary>
    public class ArrayAccess : Expression
    {
        public Expression Object { get; set; }
        public Expression Indexer { get; set; }

        [Obsolete("Used by JSON only.")]
        private ArrayAccess()
        {
            // used by JSON.
        }

        public ArrayAccess(Expression obj, Expression indexer)
        {
            Object = obj;
            Indexer = indexer;
        }
    }
}
