using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// Choices a player can make. Made up of <see cref="ChooseBlock"/>s.
    /// </summary>
    public class ChooseStatement(List<ChooseBlock> blocks) : Statement
    {
        public List<ChooseBlock> Blocks { get; set; } = blocks;
    }

    /// <summary>
    /// A single choice a player can make and the block to excecute when chosen.
    /// </summary>
    public class ChooseBlock(Expression option, Block block) : BilingualObject
    {
        public Expression Option { get; set; } = option;
        public Block Block { get; set; } = block;
    }
}
