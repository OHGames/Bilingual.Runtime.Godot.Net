using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A list of parameters.
    /// </summary>
    public class Params(List<Expression> expressions) : BilingualObject
    {
        public List<Expression> Expressions { get; set; } = expressions;
    }
}
