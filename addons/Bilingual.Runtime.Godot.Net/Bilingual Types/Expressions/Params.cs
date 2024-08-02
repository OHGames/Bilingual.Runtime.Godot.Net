using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A list of parameters.
    /// </summary>
    public class Params : BilingualObject
    {
        public List<Expression> Expressions { get; set; } = [];

        [Obsolete("Used by JSON only.")]
        private Params()
        {
            // used by JSON.
        }

        public Params(List<Expression> expressions)
        {
            Expressions = expressions;
        }
    }
}
