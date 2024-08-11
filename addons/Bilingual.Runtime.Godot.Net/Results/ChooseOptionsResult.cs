using System.Collections.Generic;

namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// Choose one of the options presented.
    /// </summary>
    public class ChooseOptionsResult(List<string> options) : BilingualResult
    {
        public override ResultType Type => ResultType.ChooseOptions;

        /// <summary>The dialogue options.</summary>
        public List<string> Options { get; } = options;
    }
}
