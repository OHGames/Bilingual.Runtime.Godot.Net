using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class IfStatement : Statement
    {
        public Expression Expression { get; set; }
        public Block Block { get; set; }
        public List<ElseIfStatement> ElseIfStatements { get; set; } = [];
        public ElseStatement? ElseStatement { get; set; }

        [Obsolete("Used by JSON only.")]
        private IfStatement()
        {
            // used by JSON.
        }

        public IfStatement(Expression expression, Block block, 
            List<ElseIfStatement> elseIfStatements, ElseStatement? elseStatement)
        {
            Expression = expression;
            Block = block;
            ElseIfStatements = elseIfStatements;
            ElseStatement = elseStatement;
        }
    }

    public class ElseIfStatement : Statement
    {
        public Expression Expression { get; set; }
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private ElseIfStatement()
        {
            // used by JSON.
        }

        public ElseIfStatement(Expression expression, Block block)
        {
            Expression = expression;
            Block = block;
        }
    }

    public class ElseStatement : Statement
    {
        public Block Block { get; set; }

        [Obsolete("Used by JSON only.")]
        private ElseStatement()
        {
            // used by JSON.
        }

        public ElseStatement(Block block)
        {
            Block = block;
        }
    }
}
