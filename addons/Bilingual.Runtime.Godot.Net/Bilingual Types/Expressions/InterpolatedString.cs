using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A string with parsable content inside.
    /// </summary>
    public class InterpolatedString : Expression
    {
        public List<Expression> Expressions { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private InterpolatedString()
        {
            // used by JSON.
        }

        public InterpolatedString(List<Expression> expressions)
        {
            Expressions = expressions;
        }
    }
}
