using System;

namespace Bilingual.Runtime.Godot.Net.Exceptions
{
    [Serializable]
    public class StatementErrorException : Exception
    {
        public StatementErrorException() { }
        public StatementErrorException(string message) : base(message) { }
        public StatementErrorException(string message, Exception inner) : base(message, inner) { }
        protected StatementErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
