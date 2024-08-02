using System;

namespace Bilingual.Runtime.Godot.Net.Exceptions
{
    /// <summary>
    /// When an error is created related to scopes and their variables.
    /// </summary>
    [Serializable]
    public class ScopeVariableException : Exception
    {
        /// <summary>
        /// When an error is created related to scopes and their variables.
        /// </summary>
        public ScopeVariableException() { }
        /// <summary>
        /// When an error is created related to scopes and their variables.
        /// </summary>
        public ScopeVariableException(string message) : base(message) { }
        /// <summary>
        /// When an error is created related to scopes and their variables.
        /// </summary>
        public ScopeVariableException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// When an error is created related to scopes and their variables.
        /// </summary>
        protected ScopeVariableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
