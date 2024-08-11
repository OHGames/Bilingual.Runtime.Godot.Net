using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.Scopes;
using Bilingual.Runtime.Godot.Net.VM;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net
{
    public partial class Test : Node2D
    {

        public override void _EnterTree()
        {
            var vm = new VirtualMachine();
            var deserializer = new Deserializer();
            var file = deserializer.DeserializeFile("res://Compiled_Scripts/test.bic", false);

            Scope.GlobalScope.Statements.AddRange(file.ScriptContainers[0].Scripts[0].Block.Statements);

            CommandStore.AddCommand($"Test.{nameof(TestCommand)}", TestCommand);
            CommandStore.AddCommand($"Test.{nameof(AsyncTestCommand)}", AsyncTestCommand);

            var line = vm.GetNextLine();
            var line1 = vm.GetNextLine();
            var line2 = vm.GetNextLine();
            var line3 = vm.GetNextLine();
            var line4 = vm.GetNextLine();
            var line5 = vm.GetNextLine();
            var line6 = vm.GetNextLine();
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
