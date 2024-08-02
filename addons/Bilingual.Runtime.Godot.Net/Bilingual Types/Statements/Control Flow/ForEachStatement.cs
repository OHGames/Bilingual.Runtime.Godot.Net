using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// A foreach loop.
    /// </summary>
    public class ForEachStatement : Statement
    {
        public string Item { get; set; }
        public Expression Collection { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private ForEachStatement()
        {
            // used by JSON.
        }

        public ForEachStatement(string item, Expression collection, Block block)
        {
            Item = item;
            Collection = collection;
            Block = block;
        }
    }
}
