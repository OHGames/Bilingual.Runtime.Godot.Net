using System;
namespace Bilingual.Runtime.Godot.Net.Exceptions
{
    /// <summary>
    /// When something goes wrong evaluating an expression.
    /// </summary>
    [Serializable]
    public class ExpressionEvaluationException : Exception
    {
        public ExpressionEvaluationException() { }
        public ExpressionEvaluationException(string message) : base(message) { }
        public ExpressionEvaluationException(string message, Exception inner) : base(message, inner) { }
        protected ExpressionEvaluationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
