using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using CLDRPlurals;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    public class LocalizedQuanity(Expression value, Dictionary<PluralCase, string> plurals, bool cardinal)  : Expression
    {
        public Expression Value { get; set; } = value;
        public Dictionary<PluralCase, string> Plurals { get; set; } = plurals;

        /// <summary>If this is getting the cardinal or the ordinal.</summary>
        public bool Cardinal { get; } = cardinal;
    }
}
