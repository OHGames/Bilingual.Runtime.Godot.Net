using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net.Commands
{
    /// <summary>
    /// An async function that can be called from a bilingual script.
    /// </summary>
    /// <param name="parameters">The functions parameters.</param>
    public delegate Task AsyncFunctionDelegate(List<object> parameters);
}
