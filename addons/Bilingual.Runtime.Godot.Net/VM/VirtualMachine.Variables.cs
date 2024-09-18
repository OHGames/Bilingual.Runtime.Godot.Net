using Bilingual.Runtime.Godot.Net.Scopes;
using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>
        /// Shorthand for <see cref="Scope.GlobalScope"/>.
        /// </summary>
        private static Scope GlobalScope => Scope.GlobalScope;

        /// <summary>
        /// Add a global variable.
        /// </summary>
        public static void AddGlobalVariable(string name, object value)
        {
            GlobalScope.AddNewVariable(name, value);
        }

        /// <summary>
        /// Add a dictionary of global variables.
        /// </summary>
        /// <param name="values">A dictionary where the key is the name and the value is the var's value.</param>
        public static void AddGlobalVariableRange(Dictionary<string, object> values)
        {
            foreach (var variable in values)
            {
                GlobalScope.AddNewVariable(variable.Key, variable.Value);
            }
        }

        /// <summary>
        /// Update a global variable.
        /// </summary>
        public static void UpdateGlobalVariableValue(string name, object newValue)
        {
            GlobalScope.UpdateVariableValue(name, newValue);
        }

        /// <summary>
        /// Remove a global variable.
        /// </summary>
        public static void RemoveGlobalVariable(string name)
        {
            GlobalScope.RemoveVariable(name);
        }

        /// <summary>
        /// Get the global scope's variables. Can be used to save data.
        /// </summary>
        /// <returns>A dictionary where the key is the var name and the value is the var's value.</returns>
        public static Dictionary<string, object> GetGlobalVariables()
        {
            return GlobalScope.Variables;
        }
    }
}
