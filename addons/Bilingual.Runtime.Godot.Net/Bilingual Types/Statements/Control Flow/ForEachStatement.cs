using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// A foreach loop.
    /// </summary>
    public class ForEachStatement(string item, Expression collection, Block block) : BlockedStatement(block)
    {
        public string Item { get; set; } = item;
        public Expression Collection { get; set; } = collection;
    }
}
