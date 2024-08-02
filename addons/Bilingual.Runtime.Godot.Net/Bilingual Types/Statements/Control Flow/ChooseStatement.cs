using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    /// <summary>
    /// Choices a player can make. Made up of <see cref="ChooseBlock"/>s.
    /// </summary>
    public class ChooseStatement : Statement
    {
        public List<ChooseBlock> Blocks { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private ChooseStatement()
        {
            // used by JSON.
        }

        public ChooseStatement(List<ChooseBlock> blocks)
        {
            Blocks = blocks;
        }
    }

    /// <summary>
    /// A single choice a player can make and the block to excecute when chosen.
    /// </summary>
    public class ChooseBlock : BilingualObject
    {
        public Expression Option { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private ChooseBlock()
        {
            // used by JSON.
        }

        public ChooseBlock(Expression option, Block block)
        {
            Option = option;
            Block = block;
        }
    }
}
