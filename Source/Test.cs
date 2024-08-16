using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.Nodes;
using Bilingual.Runtime.Godot.Net.Scopes;
using Bilingual.Runtime.Godot.Net.VM;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net
{
    public partial class Test : Node2D
    {

        public override void _Ready()
        {
            CommandStore.AddCommand($"Test.{nameof(TestCommand)}", TestCommand);
            CommandStore.AddCommand($"Test.{nameof(AsyncTestCommand)}", AsyncTestCommand);

            var runner = GetNode<DialogueRunner>("DialogueRunner");
            runner.RunScript("Test.Wow.Wow");

            var line = runner.GetNextLine();
            var line1 = runner.GetNextLine();
            var line2 = runner.GetNextLine();
            var line3 = runner.GetNextLine();
            var line4 = runner.GetNextLine();
            var line5 = runner.GetNextLine();
            var line6 = runner.GetNextLine();
        }

        public void TestCommand(List<object> parameters)
        {
            var i = parameters[0];
        }

        public async Task AsyncTestCommand(List<object> parameters)
        {
            await Task.Delay(1000);
            var i = 0;
        }
    }
}
