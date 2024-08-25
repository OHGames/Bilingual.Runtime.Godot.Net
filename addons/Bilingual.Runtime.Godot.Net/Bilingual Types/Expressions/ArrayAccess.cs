namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// When a function call or a member gets from an array.
    /// </summary>
    public class ArrayAccess(Expression obj, Expression indexer) : Expression
    {
        public Expression Object { get; set; } = obj;
        public Expression Indexer { get; set; } = indexer;
    }
}
