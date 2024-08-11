#if TOOLS
using Godot;
using Godot.Collections;

namespace Bilingual.Runtime.Godot.Net.Nodes
{
    [Tool]
    public partial class BilingualFileImporter : EditorImportPlugin
    {
        public override string _GetImporterName()
        {
            return "bilingual.script_file";
        }

        public override string _GetVisibleName()
        {
            return "Bilingual Script File";
        }

        public override string[] _GetRecognizedExtensions()
        {
            return ["bic"];
        }

        public override string _GetSaveExtension()
        {
            return "gdbic";
        }

        public override string _GetResourceType()
        {
            return nameof(BilingualFileResource);
        }

        public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
        {
            var resource = new BilingualFileResource();
            var result = ResourceSaver.Save(resource, $"{savePath}.{_GetSaveExtension()}");
            return result;
        }

        public override int _GetPresetCount()
        {
            return 1;
        }

        public override string _GetPresetName(int presetIndex)
        {
            return presetIndex switch
            {
                0 => "Default",
                _ => "Unknown"
            };
        }

        public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
        {
            return [];
        }

        public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options)
        {
            return true;
        }

        public override float _GetPriority()
        {
            return 10f;
        }

        public override int _GetImportOrder()
        {
            return 10;
        }
    }

    [Tool]
    [GlobalClass]
    public partial class BilingualResourceSaver : ResourceFormatSaver
    {
        public override string[] _GetRecognizedExtensions(Resource resource)
        {
            return ["bic"];
        }

        public override bool _Recognize(Resource resource)
        {
            return true;
        }

        public override bool _RecognizePath(Resource resource, string path)
        {
            return path.EndsWith(".gdbic");
        }

        public override Error _Save(Resource resource, string path, uint flags)
        {
            var file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead);
            if (file is null)
            {
                GD.Print($"Cant open {path} {FileAccess.GetOpenError()}");
                return Error.Failed;
            }

            var r = (BilingualFileResource)resource;
            file.StoreString(r.ResourcePath);
            file.Close();
            return file.GetError();
        }

        public override Error _SetUid(string path, long uid)
        {
            return base._SetUid(path, uid);
        }
    }

    [Tool]
    [GlobalClass]
    public partial class BilingualResourceLoader : ResourceFormatLoader
    {
        public override bool _Exists(string path)
        {
            return FileAccess.FileExists(path);
        }

        public override string[] _GetClassesUsed(string path)
        {
            return [nameof(BilingualFileResource)];
        }

        public override string[] _GetDependencies(string path, bool addTypes)
        {
            return [];
        }

        public override string[] _GetRecognizedExtensions()
        {
            return ["bic", "gdbic"];
        }

        public override string _GetResourceType(string path)
        {
            return nameof(BilingualFileResource);
        }

        public override bool _HandlesType(StringName type)
        {
            return type.ToString() == nameof(BilingualFileResource) || type.ToString() == nameof(Resource);
        }

        public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
        {
            return Variant.From(new BilingualFileResource());
        }

        public override bool _RecognizePath(string path, StringName type)
        {
            return path.EndsWith(".gdbic") || path.EndsWith(".bic");
        }
    }
}
#endif