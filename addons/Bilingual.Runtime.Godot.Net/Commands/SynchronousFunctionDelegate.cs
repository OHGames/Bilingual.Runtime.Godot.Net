using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.Commands
{
    /// <summary>
    /// A function that can be called from a bilingual script.
    /// This function call will halt the running of a script until the function is over.
    /// </summary>
    /// <param name="parameters">The function's parameters.</param>
    public delegate void SynchronousFunctionDelegate(List<object> parameters);
}
