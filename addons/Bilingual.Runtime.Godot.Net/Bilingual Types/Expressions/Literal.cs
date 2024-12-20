﻿using System.Collections;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// A literal value. String, double, etc.
    /// </summary>
    public class Literal(object value) : Expression
    {
        public object Value { get; set; } = value;

        public bool IsDouble() => Value is double;
        public bool IsBool() => Value is bool;
        public bool IsString() => Value is string;
        public bool IsList() => Value is IEnumerable;

        public static explicit operator bool(Literal lit) => (bool)lit.Value;
        public static explicit operator double(Literal lit) => (double)lit.Value;
        public static explicit operator string(Literal lit) => (string)lit.Value;
    }
}
