using CommandLine;
using System.IO;
using UpuCore;

namespace UpuConsole
{
    class UpuConsole
    {
#region Command line parameters
        [Option('i', "input", Required = true, HelpText = "Unitypackage input file.")]
        public string InputFile{ get; set; }

        [Option('o', "output", Required = false, HelpText = "The output path of the extracted unitypackage.")]
        public string OutputPath { get; set; }

        [Option('r', "register", Required = false, HelpText = "Register context menu handler")]
        public bool Register { get; set; }

        [Option('u', "unregister", Required = false, HelpText = "Unregister context menu handler")]
        public bool Unregister { get; set; }
#endregion

        internal void Start()
        {

            if (OutputPath == null && File.Exists(InputFile))
            {
                var fileInfo = new FileInfo(InputFile);
                OutputPath = Path.Combine(fileInfo.Directory.FullName, fileInfo.Name + "_unpacked");
            }

            // TODO 2: Add selective deselection via UI
            var u = new KISSUnpacker();
            if (Register)
                u.RegisterHandler();
            else if (Unregister)
                u.UnregisterHandler();

            if (InputFile != null)
                u.Unpack(InputFile, OutputPath);
        }
    }
}
