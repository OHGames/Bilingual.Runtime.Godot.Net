using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// When a function is called.
    /// </summary>
    public class FunctionCallExpression : Expression
    {
        public string Name { get; set; }
        //public List<Accessor> Accessors { get; set; } = [];
        public Params Params { get; set; }
        public bool Await { get; set; }

        [Obsolete("Used by JSON only.")]
        private FunctionCallExpression()
        {
            // used by JSON.
        }

        public FunctionCallExpression(string name, Params @params, bool async)
        {
            Name = name;
            //Accessors = accessors;
            Params = @params;
            Await = async;
        }
    }
}
