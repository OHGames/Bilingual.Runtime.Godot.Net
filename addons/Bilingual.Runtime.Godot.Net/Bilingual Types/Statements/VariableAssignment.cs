using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// Changing a variable's value.
    /// </summary>
    public class VariableAssignment : Statement
    {
        public Variable Variable { get; set; }
        public Expression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private VariableAssignment()
        {
            // used by JSON.
        }

        public VariableAssignment(Variable name, Expression expression)
        {
            Variable = name;
            Expression = expression;
        }
    }
}
