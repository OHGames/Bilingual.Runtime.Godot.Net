using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// Creating a new variable.
    /// </summary>
    public class VariableDeclaration : Statement
    {
        public string Name { get; set; }
        public bool Global { get; set; }
        public Expression Expression { get; set; }

        [Obsolete("Used by JSON only.")]
        private VariableDeclaration()
        {
            // used by JSON.
        }

        public VariableDeclaration(string name, bool global, Expression expression)
        {
            Name = name;
            Global = global;
            Expression = expression;
        }
    }
}
