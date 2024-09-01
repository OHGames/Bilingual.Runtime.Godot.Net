using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using ReswPlusLib;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    public class LocalizedQuanity(Expression value, Dictionary<PluralTypeEnum, string> plurals)  : Expression
    {
        public Expression Value { get; set; } = value;
        public Dictionary<PluralTypeEnum, string> Plurals { get; set; } = plurals;
    }
}
