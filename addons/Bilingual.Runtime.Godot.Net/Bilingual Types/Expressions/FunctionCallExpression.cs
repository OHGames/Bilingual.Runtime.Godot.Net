namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// When a function is called.
    /// </summary>
    public class FunctionCallExpression(string name, Params @params, bool async) : Expression
    {
        public string Name { get; set; } = name;
        public Params Params { get; set; } = @params;
        public bool Await { get; set; } = async;
    }
}
