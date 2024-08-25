using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Containers;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.Results;
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

        /// <summary>When the dialogue is paused.
        /// Only called when <see cref="UseVmToWait"/> is true.</summary>
        /// <param name="result">The dialogue result.</param>
        [Signal]
        public delegate void DialoguePausedEventHandler(BilingualResult result);

        /// <summary>When the dialogue is resumed.
        /// Only called when <see cref="UseVmToWait"/> is true.</summary>
        [Signal]
        public delegate void DialogueResumedEventHandler();

        /// <summary>The file paths of files to add when ready.</summary>
        [Export]
        public Array<BilingualFileResource> FilePaths { get; set; } = [];

        /// <summary>If the files are serialized using BSON.</summary>
        [Export]
        public bool UsesBson { get; set; } = false;

        /// <summary>Translation settings</summary>
        [Export]
        [ExportGroup("Translation")]
        public BilingualTranslationSettingsResource? TranslationSettings { get; set; }

        /// <summary>If the VM should wait instead of having the user
        /// handle wait statements. If this is true (which it is by default), 
        /// the timer will be created and no dialogue can be called until wait 
        /// is over. Turn this off to control the specific timing
        /// for wait statements. The VM will use the <see cref="SceneTree.CreateTimer(double, bool, bool, bool)"/> default
        /// settings for determinig the time to wait.</summary>
        public bool UseVmToWait
        {
            get => VirtualMachine.UseVmToWait;
            set => VirtualMachine.UseVmToWait = value;
        }

        public override void _Ready()
        {
            BilingualTranslationService.SetTranslation(TranslationSettings!);
            var deserializer = new Deserializer();
            foreach (var path in FilePaths)
            {
                if (path is null) continue;

                var file = deserializer.DeserializeFile(path.FilePath, UsesBson);
                AddFile(file);
            }

            EmitSignal(SignalName.ScriptsLoaded);
        }

        protected override void Dispose(bool disposing)
        {
            BilingualTranslationService.CloseScripts();
            base.Dispose(disposing);
        }

        public override void _EnterTree()
        {
            VirtualMachine.tree = GetTree();

            // The VM is not a node so just call these functions.
            VirtualMachine.PausedCallback += CallPausedCallback;
            VirtualMachine.ResumedCallback += CallResumedCallback;
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
                container.Scripts.ForEach(s =>
                {
                    var name = GetFullName(container, s);
                    VirtualMachine.Scripts.TryAdd(name, s);
                    AddScriptToTranslation(name);
                });
                
            }
        }

        /// <summary>Get the full name seperated by '.'</summary>
        private string GetFullName(ScriptContainer scriptContainer, Script script)
        {
            return scriptContainer.Name + "." + script.Name;
        }

        /// <summary>
        /// Run a script. Call <see cref="GetNextLine"/> to get dialogue.
        /// </summary>
        /// <param name="name"></param>
        public void RunScript(string name) => VirtualMachine.LoadScript(name);

        /// <summary>
        /// Return the next line of dialogue from the currently loaded script.
        /// </summary>
        /// <returns>A result.</returns>
        public BilingualResult GetNextLine() => VirtualMachine.GetNextLine();

        /// <summary>Add a new callback for when dialogue is paused.
        /// Only called when <see cref="UseVmToWait"/> is true.
        /// Signals are not used because this class is not a Node.</summary>
        /// <param name="paused">The paused callback function.</param>
        public void CallPausedCallback(BilingualResult result) 
            => EmitSignal(SignalName.DialoguePaused, result);

        /// <summary>Add a new callback for when dialogue is resumed.
        /// Only called when <see cref="UseVmToWait"/> is true.
        /// Signals are not used because this class is not a Node.</summary>
        /// <param name="action">The resumed callback function.</param>
        private void CallResumedCallback()
            => EmitSignal(SignalName.DialogueResumed);

        /// <summary>Set the translation.</summary>
        private void SetTranslation()
        {
            BilingualTranslationService.SetTranslation(TranslationSettings!);
        }

        /// <summary>When the translation has changed, files need to be re-added.</summary>
        public void SetTranslationAndAddFiles()
        {
            SetTranslation();
            foreach (var file in VirtualMachine.Scripts)
            {
                AddScriptToTranslation(file.Key);
            }
        }

        /// <summary>Add script to translation service.</summary>
        /// <param name="script">The script to add.</param>
        public void AddScriptToTranslation(string script)
        {
            BilingualTranslationService.AddScript(script);
        }
    }
}
