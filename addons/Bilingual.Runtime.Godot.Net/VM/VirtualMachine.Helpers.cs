using Bilingual.Runtime.Godot.Net.BilingualTypes;
using System.Collections;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>If the operator is a conditional operator (returns true/false).</summary>
        public static bool IsConditionalOperator(Operator opr)
        {
            return opr == Operator.EqualTo || opr == Operator.NotEqualTo
                || opr == Operator.GreaterThanEqualTo || opr == Operator.LessThanEqualTo
                || opr == Operator.GreaterThan || opr == Operator.LessThan;
        }

        /// <summary>If the operator is a math operator (returns a number).</summary>
        public static bool IsMathOperator(Operator opr)
        {
            return opr == Operator.Pow || opr == Operator.Mul
                || opr == Operator.Div || opr == Operator.Mod
                || opr == Operator.Add || opr == Operator.Sub
                || opr == Operator.PlusPlus || opr == Operator.MinusEqual
                || opr == Operator.MulEqual || opr == Operator.DivEqual
                || opr == Operator.PlusEqual || opr == Operator.MinusEqual;
        }

        /// <summary>Check if double is a whole number.</summary>
        /// <param name="d">The double.</param>
        /// <returns>True if whole.</returns>
        public static bool IsWholeNumber(double d)
        {
            // When casted to an int, the decimals are truncated (cut off).
            // So if the value of i does not equal d, it means that there was a truncated decimal.
            long i = (long)d;
            return i == d;
        }


        /// <summary>Convert an object to a string.</summary>
        /// <param name="value">The value of the expression.</param>
        /// <returns>A string representation of the object.</returns>
        private static string ValueToString(object value)
        {
            if (value is null) return "null";

            if (value is string str)
            {
                // catch a string before the next if statement because
                // strings are also enumerable and we dont want to
                // convert a string to an array of chars.
                return str;
            }
            else if (value is IEnumerable objects)
            {
                var listStr = "[";
                foreach (var item in objects)
                {
                    listStr += item.ToString() + ", ";
                }
                // cut off the last item's comma and space.
                listStr = listStr[..^2] + "]";
                return listStr;
            }
            else
            {
                return value.ToString() ?? "null";
            }
        }

    }
}
