using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;
using System.Collections.Generic;
using System;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A choose block.
    /// </summary>
    public class ChooseScope : BlockedScope<ChooseStatement>
    {
        /// <summary>The choose blocks in the statement.</summary>
        public List<ChooseBlock> Blocks => Statement.Blocks;

        /// <summary>The list of options and their blocks.</summary>
        private readonly Dictionary<string, Block> options = [];

        /// <summary>If an option has been selected yet.</summary>
        private bool selectedOption;

        public ChooseScope(Scope? parentScope, VirtualMachine virtualMachine, ChooseStatement chooseStatement) 
            : base(parentScope, virtualMachine, chooseStatement)
        {
            foreach (var block in Blocks)
            {
                // Options are strings so convert any non-string objects to a string.
                options.Add(VirtualMachine.EvaluateExpression(block.Option).ToString() ?? "", block.Block);
            }
        }

        /// <summary>Get the options for the choose block.</summary>
        /// <returns>A list of options.</returns>
        public List<string> GetOptions() => [.. options.Keys];

        /// <summary>Choose the option and set the statements for this scope.</summary>
        /// <param name="option">The option to select.</param>
        /// <exception cref="ArgumentException">If the option is not in the list of options.</exception>
        public void SelectOption(string option)
        {
            if (!options.TryGetValue(option, out Block? block))
            {
                throw new ArgumentException("Option does not exist in list.", nameof(option));
            }

            Statements = block.Statements;
            selectedOption = true;
        }

        public override Statement? GetNextStatement()
        {
            if (!selectedOption) throw new InvalidOperationException("Did not select an option.");
            return base.GetNextStatement();
        }
    }
}
