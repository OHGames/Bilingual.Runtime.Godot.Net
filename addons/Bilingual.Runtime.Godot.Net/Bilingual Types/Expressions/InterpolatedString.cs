using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A string with parsable content inside.
    /// </summary>
    public class InterpolatedString(List<Expression> expressions) : Expression
    {
        public List<Expression> Expressions { get; set; } = expressions;
    }
}
