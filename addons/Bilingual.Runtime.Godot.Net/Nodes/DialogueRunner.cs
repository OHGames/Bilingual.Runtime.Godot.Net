using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.VM;
using Godot;
using Godot.Collections;

// There is a conflict between Godot scripts and bilingual scripts.
using Script = Bilingual.Runtime.Godot.Net.BilingualTypes.Containers.Script;

namespace Bilingual.Runtime.Godot.Net.Nodes
{
    /// <summary>
    /// The Dialogue Runner is a <see cref="Node"/> uses the <see cref="VirtualMachine"/> to run dialogue.
    /// </summary>
    [GlobalClass]
    [Icon("res://addons/Bilingual.Runtime.Godot.Net/Assets/bubble.svg")]
    public partial class DialogueRunner : Node
    {
        /// <summary>The virtual machine.</summary>
        public VirtualMachine VirtualMachine { get; private set; } = new VirtualMachine();

        /// <summary>When all the scripts inside of <see cref="FilePaths"/> 
        /// have been loaded.</summary>
        [Signal]
        public delegate void ScriptsLoadedEventHandler();

        /// <summary>The file paths of files to add when ready.</summary>
        [Export]
        public Array<BilingualFileResource> FilePaths { get; set; } = [];

        /// <summary>If the files are serialized using BSON.</summary>
        [Export]
        public bool UsesBson { get; set; } = false;

        public override void _Ready()
        {
            var deserializer = new Deserializer();
            foreach (var path in FilePaths)
            {
                if (path is null) continue;

                var file = deserializer.DeserializeFile(path.ResourcePath, UsesBson);
                AddFile(file);
            }

            EmitSignal(SignalName.ScriptsLoaded);
        }

        /// <summary>Add a script. Scripts must be added in order to be run first.
        /// Files in <see cref="FilePaths"/> are added automatically.</summary>
        /// <param name="script">The script to add.</param>
        public void AddScript(Script script)
        {
            VirtualMachine.Scripts.TryAdd(script.Name, script);
        }

        /// <summary>Add a file and its scripts. Files and scripts must be added in order to be run first.
        /// Files in <see cref="FilePaths"/> are added automatically.</summary>
        /// <param name="file"></param>
        public void AddFile(BilingualFile file)
        {
            foreach (var container in file.ScriptContainers)
            {
                container.Scripts.ForEach(s => VirtualMachine.Scripts.TryAdd(s.Name, s));
            }
        }
    }
}
