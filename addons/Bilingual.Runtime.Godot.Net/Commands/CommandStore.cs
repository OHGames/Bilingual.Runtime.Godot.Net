using System;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.Commands
{
    /// <summary>
    /// Stores all the commands.
    /// </summary>
    public static class CommandStore
    {
        /// <summary>
        /// A list of commands.
        /// </summary>
        public static Dictionary<string, SynchronousFunctionDelegate> Commands { get; } = [];

        /// <summary>
        /// A list of async commands.
        /// </summary>
        public static Dictionary<string, AsyncFunctionDelegate> AsyncCommands { get; } = [];

        /// <summary>
        /// A list of syncronous inline commands.
        /// </summary>
        public static Dictionary<string, InlineSyncronousFuncDelegate> InlineCommands { get; } = [];

        /// <summary>
        /// Add a syncronous command.
        /// </summary>
        /// <param name="name">The name (with possible namespace + class).</param>
        /// <param name="function">The function.</param>
        public static void AddCommand(string name, SynchronousFunctionDelegate function)
        {
            var success = Commands.TryAdd(name, function);
            if (!success) throw new InvalidOperationException($"Command, {name}, already added.");
        }

        /// <summary>
        /// Add an asyncronous command.
        /// </summary>
        /// <param name="name">The name (with possible namespace + class).</param>
        /// <param name="function">The function.</param>
        public static void AddCommand(string name, AsyncFunctionDelegate function)
        {
            var success = AsyncCommands.TryAdd(name, function);
            if (!success) throw new InvalidOperationException($"Command, {name}, already added.");
        }

        /// <summary>
        /// Add a syncronous inline command.
        /// </summary>
        /// <param name="name">The name (with possible namespace + class).</param>
        /// <param name="func">The function.</param>
        public static void AddInlineCommand(string name, InlineSyncronousFuncDelegate func)
        {
            var success = InlineCommands.TryAdd(name, func);
            if (!success) throw new InvalidOperationException($"Command, {name}, already added.");
        }
    }
}
