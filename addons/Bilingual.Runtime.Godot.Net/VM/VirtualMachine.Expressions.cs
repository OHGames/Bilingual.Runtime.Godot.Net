using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.Exceptions;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bilingual.Runtime.Godot.Net.VM
{
    /// <summary>
    /// The Virtual Machine excecutes the script and returns the dialogue.
    /// </summary>
    public partial class VirtualMachine
    {
        /// <summary>Evaluate an expression and get its value.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The value of the expression.</returns>
        [return: NotNull]
        public object EvaluateExpression(Expression expression)
        {
            if (expression is Literal lit)
            {
                if (lit.Value is ICollection collection)
                {
                    List<object> objects = [];
                    foreach (var obj in collection)
                    {
                        // When deserializing, the value is a JObject for arrays.
                        // TODO: figure out why. This below is a fix that just evaluates
                        // each JObject and returns it.
                        if (obj is JObject jObj)
                        {
                            var serializer = new JsonSerializer();
                            serializer.Converters.Add(Deserializer.serializerSettings.Converters.Where(c => c is JsonSubtypes).First());
                            objects.Add(EvaluateExpression(jObj.ToObject<Expression>(serializer)));
                        }
                        else
                        {
                            objects.Add(EvaluateExpression((Expression)obj));
                        }
                    }
                    return objects;
                }

                return lit.Value;
            }
            else if (expression is OprExpression opr)
            {
                if (IsConditionalOperator(opr.Operator))
                    return EvaluateConditionalExpression(opr);
                else
                    return EvaluateMathExpression(opr);
            }
            else if (expression is Variable variable)
            {
                if (CurrentScope is null) throw new ExpressionEvaluationException("Current scope is null");

                // TODO: accessors
                return CurrentScope.GetVariableValue(variable);
            }
            else if (expression is InterpolatedString interpolated)
            {
                // RunInterpolatedDialogue will evaluate this so just return the object.
                return interpolated;
            }

            throw new InvalidExpressionException("Cannot evaluate a value for this expression.");
        }

        /// <summary>Evaluate an expression and get its value.
        /// Will throw a C# exception if the object is not <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The desired return type.</typeparam>
        /// <param name="exp">The expression to evaluate.</param>
        /// <returns>The object as <typeparamref name="T"/>.</returns>
        public T EvaluateExpression<T>(Expression exp) => (T)EvaluateExpression(exp);

        /// <summary>Evaluate a conditional expression.</summary>
        /// <param name="expression">The conditional expression.</param>
        /// <returns>A bool with the expression's value.</returns>
        /// <exception cref="ExpressionEvaluationException">When expression is not conditional.</exception>
        public bool EvaluateConditionalExpression(Expression expression)
        {
            if (expression is OprExpression opr)
            {
                var left = opr.Left;
                var right = opr.Right;

                // binary expression
                if (left != null && right != null)
                {
                    return EvaluateBinaryConditional(opr);
                }
                else
                {
                    return (bool)EvaluateUnaryExpression(opr);
                }
            }
            else if (expression is Variable variable)
            {
                if (CurrentScope is null) throw new ExpressionEvaluationException("Current scope is null");

                return (bool)CurrentScope.GetVariableValue(variable);
            }

            throw new ExpressionEvaluationException("Not a conditional expression.");
        }

        /// <summary>Evaluate a math expression. Can be binary or unary.</summary>
        /// <param name="expression">The math expression.</param>
        /// <returns>A double with the value.</returns>
        /// <exception cref="ExpressionEvaluationException">If the expression is not a math expression.</exception>
        public object EvaluateMathExpression(Expression expression)
        {
            if (expression is OprExpression opr)
            {
                if (opr.Left != null && opr.Right != null)
                {
                    return EvaluateBinaryMathExpression(opr);
                }
                else
                {
                    return EvaluateUnaryExpression(opr);
                }
            }
            else if (expression is Variable variable)
            {
                if (CurrentScope is null) throw new ExpressionEvaluationException("Current scope is null");

                return CurrentScope.GetVariableValue(variable);
            }

            throw new ExpressionEvaluationException("Not a math expression.");
        }

        /// <summary>Evaluate a binary conditional.</summary>
        /// <param name="opr">The binary expression.</param>
        /// <returns>A bool with the expression's value.</returns>
        /// <exception cref="ExpressionEvaluationException">When the expression is not a binary conditional.</exception>
        public bool EvaluateBinaryConditional(OprExpression opr)
        {
            var oprLeft = opr.Left ?? throw new ExpressionEvaluationException("Not unary");
            var oprRight = opr.Right ?? throw new ExpressionEvaluationException("Not unary");

            var left = EvaluateExpression(oprLeft);
            var right = EvaluateExpression(oprRight);

            if (opr.Operator == Operator.EqualTo || opr.Operator == Operator.NotEqualTo)
            {
                var equal = AreTwoValuesEqual(left, right, out bool unhandledType);
                if (!unhandledType)
                {
                    return equal;
                }
            }

            return opr.Operator switch
            {
                // unhandled type, try to compare them.
                Operator.EqualTo => left == right,
                Operator.NotEqualTo => left != right,

                // these operators will always have doubles so just cast
                Operator.GreaterThanEqualTo => (double)left >= (double)right,
                Operator.LessThanEqualTo => (double)left <= (double)right,
                Operator.GreaterThan => (double)left > (double)right,
                Operator.LessThan => (double)left < (double)right,

                _ => throw new ExpressionEvaluationException("Operator is not a binary conditional")
            };
        }

        /// <summary>The objects need to be casted to their actual types to see
        /// if they are equal. Doubles represented by an <c>object</c> will not check if the value is equal,
        /// the reference will be checked instead. This function will cast the values to their 
        /// actual types and then compare to get a true comparison.</summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="unhandledType">If the types were not the same.</param>
        /// <returns>True if the values are equal.</returns>
        private bool AreTwoValuesEqual(object left, object right, out bool unhandledType)
        {
            unhandledType = false;

            // Different types will not be equal.
            if (left.GetType() != right.GetType())
            {
                return false;
            }

            switch (left)
            {
                case double lD:
                    return lD == (double)right;

                case string lS:
                    return lS == (string)right;

                case IEnumerable lE:
                    // loop through and compare the value of the objects.
                    //var rE = (IEnumerable)right;
                    //var rEnumerator = rE.GetEnumerator();
                    //foreach (var item in lE)
                    //{
                    //    if (!rEnumerator.MoveNext()) return false;

                    //    if (AreTwoValuesEqual(item, rEnumerator.Current, out bool _))
                    //        continue;
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}
                    //return true;
                    var listLeft = (List<object>)left;
                    var listRight = (List<object>)right;
                    if (listLeft.Count != listRight.Count) return false;
                    for (int i = 0; i < listLeft.Count; i++)
                    {
                        var itemLeft = listLeft[i];
                        var itemRight = listRight[i];
                        if (AreTwoValuesEqual(itemLeft, itemRight, out bool _))
                            continue;
                        else
                            return false;
                    }
                    return true;

                default:
                    unhandledType = true;
                    return false;
            }
        }

        /// <summary>Evaluate a binary math expression. String addition is considered math.</summary>
        /// <param name="opr">The binary expression.</param>
        /// <returns>The value of the expression. Either a double or string.</returns>
        /// <exception cref="ExpressionEvaluationException">
        /// If the expression is not a binary math expression or a string is added to a non-string.</exception>
        public object EvaluateBinaryMathExpression(OprExpression opr)
        {
            if (opr.Left == null || opr.Right == null) 
                throw new ExpressionEvaluationException("Expression is not binary.");

            var left = EvaluateExpression(opr.Left);
            var right = EvaluateExpression(opr.Right);
            var isAddition = opr.Operator == Operator.Add;

            // both are strings
            if (left is string lStr && right is string rStr && isAddition)
            {
                return lStr + rStr;
            }
            // if only one is a string
            else if (((left is string && right is not string) || (left is not string && right is string)) && isAddition)
            {
                throw new ExpressionEvaluationException("Strings can only be added to other strings.");
            }

            // only doubles now
            var leftDouble = (double)left;
            var rightDouble = (double)right;

            return opr.Operator switch
            {
                Operator.Pow => Math.Pow(leftDouble, rightDouble),
                Operator.Mul => leftDouble * rightDouble,
                Operator.Div => leftDouble / rightDouble,
                Operator.Mod => leftDouble % rightDouble,
                Operator.Add => leftDouble + rightDouble,
                Operator.Sub => leftDouble - rightDouble,

                Operator.MulEqual => EvaluateMulDivPlusMinusExpr(opr),
                Operator.DivEqual => EvaluateMulDivPlusMinusExpr(opr),
                Operator.PlusEqual => EvaluateMulDivPlusMinusExpr(opr),
                Operator.MinusEqual => EvaluateMulDivPlusMinusExpr(opr),

                _ => throw new ExpressionEvaluationException("Expression is not a binary math expression.")
            };
        }

        /// <summary>Evaluate a <see cref="BilingualTypes.Statements.PlusMinusMulDivEqualStatement"/> expression.</summary>
        /// <param name="opr">The expression to evaluate.</param>
        /// <returns>The new value of the variable.</returns>
        /// <exception cref="ExpressionEvaluationException">When a string is added to a non-string.</exception>
        public object EvaluateMulDivPlusMinusExpr(OprExpression opr)
        {
            var variable = (Variable)opr.Left!;
            var right = EvaluateExpression(opr.Right!);

            if (CurrentScope is null) throw new ExpressionEvaluationException("Current scope is null");

            var varValue = CurrentScope.GetVariableValue(variable);
            var isAddition = opr.Operator == Operator.PlusEqual;

            // if only one is a string and adding
            if (((varValue is string && right is not string) || (varValue is not string && right is string)) && isAddition)
            {
                throw new ExpressionEvaluationException("Can only add strings to other strings.");
            }
            else if (varValue is string strValue && isAddition)
            {
                strValue += (string)right;
                CurrentScope.UpdateVariableValue(variable.Name, strValue);
                return strValue;
            }

            // expression is between doubles.
            var variableDouble = (double)varValue;
            var doubleRight = (double)right;

            switch (opr.Operator)
            {
                case Operator.MulEqual:
                    variableDouble *= doubleRight;
                    break;
                case Operator.DivEqual:
                    variableDouble /= doubleRight;
                    break;
                case Operator.PlusEqual:
                    variableDouble += doubleRight;
                    break;
                case Operator.MinusEqual:
                    variableDouble -= doubleRight;
                    break;
                default:
                    throw new ExpressionEvaluationException("Expression is not a MullDivPlusMinusEqual expression.");
            }

            CurrentScope.UpdateVariableValue(variable.Name, variableDouble);
            return variableDouble;
        }

        /// <summary>Evaluate any type of unary expression.</summary>
        /// <param name="opr">The unary expression.</param>
        /// <returns>The value of the unary expression.</returns>
        /// <exception cref="ExpressionEvaluationException">When the expression is not unary.</exception>
        public object EvaluateUnaryExpression(OprExpression opr)
        {
            var oprLeft = opr.Left;
            var oprRight = opr.Right;

            // only one side will be null
            var sideToEvaluate = oprLeft ?? oprRight;
            var side = EvaluateExpression(sideToEvaluate!);

            return opr.Operator switch
            {
                Operator.PlusPlus => EvaluatePlusPlusMinusMinus(opr),
                Operator.MinusMinus => EvaluatePlusPlusMinusMinus(opr),

                Operator.Bang => !(bool)side,
                Operator.Add => Math.Abs((double)side),
                Operator.Sub => -(double)side,

                _ => throw new ExpressionEvaluationException("Operator is not a unary operator.")
            };
        }

        /// <summary>Evaluate a plusplus or minusminus expression.</summary>
        /// <param name="opr">The expression.</param>
        /// <returns>If operator is on the left, returns the updated value. Otherwise, returns value before change. 
        /// The actual variable value is updated in the scope regardless of operator side.</returns>
        /// <exception cref="ExpressionEvaluationException">If operands are not doubles or 
        /// the incorrect operator is used.</exception>
        public object EvaluatePlusPlusMinusMinus(OprExpression opr)
        {
            var oprOnLeft = opr.Left == null;
            var variable = oprOnLeft ? (Variable)opr.Right! : (Variable)opr.Left!;

            if (CurrentScope is null) throw new ExpressionEvaluationException("Current scope is null");

            var varValue = CurrentScope.GetVariableValue(variable);

            if (varValue is not double) 
                throw new ExpressionEvaluationException("Plusplus or minusminus must be a double.");

            var varValueDouble = (double)varValue;

            // if opr is on the left, return the incramented/decramented value.
            // if on the right, return original but update the value.
            switch (opr.Operator)
            {
                case Operator.PlusPlus:
                    CurrentScope.UpdateVariableValue(variable, varValueDouble + 1);
                    return oprOnLeft ? varValueDouble + 1 : varValueDouble;
                case Operator.MinusMinus:
                    CurrentScope.UpdateVariableValue(variable, varValueDouble - 1);
                    return oprOnLeft ? varValueDouble - 1 : varValueDouble;
                default:
                    throw new ExpressionEvaluationException("Not a plusplus or minusminus expression.");
            }
        }
    }
}

