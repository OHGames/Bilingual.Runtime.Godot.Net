using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.Commands
{
    /// <summary>
    /// An inline function that can be called from inside 
    /// a <see cref="BilingualTypes.Expressions.InterpolatedString"/>.
    /// </summary>
    /// <param name="parameters">The parameters of the function.</param>
    /// <returns>A string to be inserted into the interpolated string.</returns>
    public delegate string InlineSyncronousFuncDelegate(List<object> parameters);
}
