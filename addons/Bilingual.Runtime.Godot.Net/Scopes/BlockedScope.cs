using Bilingual.Runtime.Godot.Net.VM;

namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// A scope that has a statement and its block.
    /// </summary>
    /// <typeparam name="TStmt">The statement type.</typeparam>
    /// <param name="parentScope">The parent scope.</param>
    /// <param name="virtualMachine">The VM.</param>
    /// <param name="stmt">The statement this scope uses.</param>
    public class BlockedScope<TStmt>(Scope? parentScope, VirtualMachine virtualMachine, TStmt stmt)
        : Scope(parentScope, virtualMachine)
    {
        public TStmt Statement { get; } = stmt;
    }
}
