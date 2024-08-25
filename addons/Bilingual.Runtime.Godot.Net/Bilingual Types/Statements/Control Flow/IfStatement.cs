using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class IfStatement(Expression expression, Block block,
        List<ElseIfStatement> elseIfStatements, ElseStatement? elseStatement) : Statement
    {
        public Expression Expression { get; set; } = expression;
        public Block Block { get; set; } = block;
        public List<ElseIfStatement> ElseIfStatements { get; set; } = elseIfStatements;
        public ElseStatement? ElseStatement { get; set; } = elseStatement;
    }

    public class ElseIfStatement(Expression expression, Block block) : Statement
    {
        public Expression Expression { get; set; } = expression;
        public Block Block { get; set; } = block;
    }

    public class ElseStatement(Block block) : Statement
    {
        public Block Block { get; set; } = block;
    }
}
