namespace Bilingual.Runtime.Godot.Net.Scopes
{
    /// <summary>
    /// If the scope can be broken out of or continued.
    /// </summary>
    public interface IBreakableScope
    {
        /// <summary>
        /// Break out the scope.
        /// </summary>
        public void Break();

        /// <summary>
        /// Continue to next iteration.
        /// </summary>
        public void Continue();
    }
}
