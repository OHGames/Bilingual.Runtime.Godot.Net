using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.VM;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A foreach loop.
    /// </summary>
    public class ForEachScope : BlockedScope<ForEachStatement>, IBreakableScope
    {
        /// <summary>The name of the new variable.</summary>
        public string ItemName => Statement.Item;

        /// <summary>The collection to loop through.</summary>
        public Expression Collection => Statement.Collection;

        /// <summary>At the end of the block and we can move on to next item.</summary>
        private bool endOfLoop;

        /// <summary>If loop just started.</summary>
        private bool firstLoop = true;

        /// <summary>The actual collection to loop through.</summary>
        private IEnumerable collection;

        /// <summary>The <see cref="IEnumerator"/> of <see cref="collection"/>.</summary>
        private IEnumerator enumerator;

        private bool broken = false;
        private bool continueLoop = false;

        public ForEachScope(Scope? parentScope, VirtualMachine virtualMachine, ForEachStatement forEachStatement) 
            : base(parentScope, virtualMachine, forEachStatement)
        {
            Statements = forEachStatement.Block.Statements;
            loopParent = this;
        }

        public override Statement? GetNextStatement()
        {
            if (broken) return null;
            if (continueLoop)
            {
                if (!MoveNext()) return null;
                continueLoop = false;
            }

            if (firstLoop)
            {
                firstLoop = false;
                collection = VirtualMachine.EvaluateExpression<IEnumerable>(Collection);
                enumerator = collection.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    AddNewVariable(ItemName, enumerator.Current);
                }
                else
                {
                    return null;
                }
            }

            var line = base.GetNextStatement();
            endOfLoop = line is null;

            if (line is BreakStatement) return null;
            if (line is ContinueStatement) endOfLoop = true;

            if (endOfLoop)
            {
                if (!MoveNext()) return null;

                currentStatement = 0;
                return GetNextStatement();
            }

            return line;
        }

        private bool MoveNext()
        {
            if (!enumerator.MoveNext()) return false;
            UpdateVariableValue(ItemName, enumerator.Current);
            return true;
        }

        public void Break()
        {
            broken = true;
        }

        public void Continue()
        {
            endOfLoop = true;
            currentStatement = 0;
            continueLoop = true;
        }
    }
}
