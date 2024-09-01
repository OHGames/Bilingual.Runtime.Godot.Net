namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    public class BlockedStatement(Block block) : Statement
    {
        public Block Block { get; set; } = block;
    }
}
