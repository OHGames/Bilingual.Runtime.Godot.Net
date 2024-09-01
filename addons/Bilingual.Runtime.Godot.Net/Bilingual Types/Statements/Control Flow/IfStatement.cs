using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow
{
    public class IfStatement(Expression expression, Block block,
        List<ElseIfStatement> elseIfStatements, ElseStatement? elseStatement) : BlockedStatement(block)
    {
        public Expression Expression { get; set; } = expression;
        public List<ElseIfStatement> ElseIfStatements { get; set; } = elseIfStatements;
        public ElseStatement? ElseStatement { get; set; } = elseStatement;
    }

    public class ElseIfStatement(Expression expression, Block block) : BlockedStatement(block)
    {
        public Expression Expression { get; set; } = expression;
    }

    public class ElseStatement(Block block) : BlockedStatement(block)
    {
    }
}
