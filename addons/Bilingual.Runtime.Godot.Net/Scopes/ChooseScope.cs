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
        private readonly List<Block> optionsBlocks = [];

        /// <summary>The option texts.</summary>
        private readonly List<string> optionTexts = [];

        /// <summary>If an option has been selected yet.</summary>
        private bool selectedOption;

        public ChooseScope(Scope? parentScope, VirtualMachine virtualMachine, ChooseStatement chooseStatement) 
            : base(parentScope, virtualMachine, chooseStatement)
        {
            foreach (var block in Blocks)
            {
                optionsBlocks.Add(block.Block);
                // Options are strings so convert any non-string objects to a string.
                optionTexts.Add(VirtualMachine.EvaluateExpression(block.Option).ToString() ?? "");
            }
        }

        /// <summary>Get the options for the choose block.</summary>
        /// <returns>A list of options.</returns>
        public List<string> GetOptions() => optionTexts;

        /// <summary>Choose the option and set the statements for this scope.</summary>
        /// <param name="index">The index of the option to select.</param>
        /// <exception cref="ArgumentException">If the option is not in the list of options.</exception>
        public void SelectOption(int index)
        {
            if (!CheckIndex(index))
            {
                throw new ArgumentException("Option does not exist in list.", nameof(index));
            }

            Statements = Blocks[index].Block;
            selectedOption = true;
        }

        public override Statement? GetNextStatement()
        {
            if (!selectedOption) throw new InvalidOperationException("Did not select an option.");
            return base.GetNextStatement();
        }

        private bool CheckIndex(int i)
        {
            return i > 0 || i < Blocks.Count;
        }
    }
}
