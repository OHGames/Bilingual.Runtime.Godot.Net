using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.Exceptions;
using Bilingual.Runtime.Godot.Net.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A scope is a block of statements that also has their own variables.
    /// </summary>
    /// <param name="parentScope">The parent scope.</param>
    public class Scope(Scope? parentScope, VirtualMachine virtualMachine)
    {
        /// <summary>The current scope's variables.</summary>
        public Dictionary<string, object> Variables { get; } = [];

        /// <summary>The parent scope.</summary>
        public Scope? ParentScope { get; set; } = parentScope;

        /// <summary>This scope's children.</summary>
        public List<Scope> ChildScopes { get; set; } = [];

        /// <summary>The global scope. It has no parent. 
        /// This is the most distant <see cref="ParentScope"/> for all scopes.
        /// The Virtual Machine will be set later.</summary>
        public static Scope GlobalScope { get; } = new Scope(null, null!);

        /// <summary>The statements within this scope.</summary>
        public List<Statement> Statements { get; protected set; } = [];

        /// <summary>Current statement index.</summary>
        protected int currentStatement;

        /// <summary>The virtual machine this scope is a part of.</summary>
        public VirtualMachine VirtualMachine { get; set; } = virtualMachine;

        /// <summary>Get the next statement by incrementing the current index.</summary>
        /// <returns>The next statement. Returns null when scope is over.</returns>
        public virtual Statement? GetNextStatement()
        {
            var statement = Statements.ElementAtOrDefault(currentStatement++);
            if (statement is not null)
            {
                if (statement is BreakStatement) return null;
            }

            return statement;
        }

        /// <summary>Add a new variable.</summary>
        /// <exception cref="Exception">If variable exists already.</exception>
        public void AddNewVariable(string name, object value)
        {
            if (VariableNameExists(name, out _, out Scope _)) 
                throw new ScopeVariableException($"The variable, {name}, already exists.");

            Variables.Add(name, value);
        }

        /// <summary>Change the variable's value.</summary>
        /// <exception cref="Exception">If the variable does not exist.</exception>
        public void UpdateVariableValue(string name, object value)
        {
            if (!VariableNameExists(name, out _, out Scope scope))
                throw new ScopeVariableException($"The variable, {name}, does not exist.");

            scope.Variables[name] = value;
        }

        /// <summary>Change the variable's value.</summary>
        /// <exception cref="Exception">If the variable does not exist.</exception>
        public void UpdateVariableValue(Variable variable, object value) => UpdateVariableValue(variable.Name, value);

        /// <summary>Get the variable's value.</summary>
        /// <param name="name">Name of variable.</param>
        /// <returns>The variable's value.</returns>
        /// <exception cref="Exception">If the variable does not exist.</exception>
        public object GetVariableValue(string name)
        {
            if (!VariableNameExists(name, out object value, out Scope _))
                throw new ScopeVariableException($"The variable, {name}, does not exist.");

            return value;
        }

        /// <summary>Get the variable's value.</summary>
        /// <param name="name">Name of variable.</param>
        /// <returns>The variable's value.</returns>
        /// <exception cref="Exception">If the variable does not exist.</exception>
        public object GetVariableValue(Variable variable) => GetVariableValue(variable.Name);

        /// <summary>See if a variable name exists already by crawling up the scope tree.</summary>
        /// <param name="name">The variable name to check.</param>
        /// <returns>True if name exists. False otherwise.</returns>
        public bool VariableNameExists(string name, out object value, out Scope scope)
        {
            scope = this;
            if (Variables.TryGetValue(name, out value!)) return true;

            // crawl up until the global scope is reached.
            if (ParentScope != null)
                return ParentScope.VariableNameExists(name, out value, out scope);

            // Variable does not exist.
            return false;
        }
    }
}
