using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A statement in which a variable uses compound assignemnt <c>(+= -= *= /=)</c>
    /// </summary>
    public class PlusMinusMulDivEqualStatement : Statement
    {
        public OprExpression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private PlusMinusMulDivEqualStatement()
        {
            // used by JSON.
        }

        public PlusMinusMulDivEqualStatement(OprExpression expression)
        {
            Expression = expression;
        }
    }
}
