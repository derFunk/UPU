using System;
using CommandLine;
using System.IO;
using CommandLine.Text;
using UpuCore;

namespace UpuConsole
{
    class UpuConsole
    {
#region Command line parameters
        [Option('i', "input", Required = false, HelpText = "Unitypackage input file.")]
        public string InputFile{ get; set; }

        [Option('o', "output", Required = false, HelpText = "The output path of the extracted unitypackage.")]
        public string OutputPath { get; set; }

        [Option('r', "register", Required = false, HelpText = "Register context menu handler")]
        public bool Register { get; set; }

        [Option('u', "unregister", Required = false, HelpText = "Unregister context menu handler")]
        public bool Unregister { get; set; }
#endregion

        /// <summary>
        /// Starts the Unitypackage Unpacker. Unpacks package, registers or unregisters shell handler.
        /// </summary>
        internal void Start()
        {
            if (string.IsNullOrEmpty(InputFile) && !Register && !Unregister)
            {
                Console.WriteLine(GetUsage());
                Environment.Exit(1);
            }

            // if input file is given, but does not exists, exit with error
            if (!string.IsNullOrEmpty(InputFile) && !File.Exists(InputFile))
            {
                Console.WriteLine("File not found: " + InputFile);
                Console.WriteLine(GetUsage());
                Environment.Exit(2);
            }

            // If inputfile is set, we want to unpack something.
            // If its not set, we propably just want to un/register the shell handler
            if (!string.IsNullOrEmpty(InputFile))
            {
                var inputFileInfo = new FileInfo(InputFile);

                // If output path is not set, define a standard, which is <inputfilepath_unpacked>
                if (OutputPath == null)
                    OutputPath = Path.Combine(inputFileInfo.Directory.FullName, inputFileInfo.Name + "_unpacked");

                // If output path already exists, find an alternative!
                if (Directory.Exists(OutputPath))
                {
                    int appendix = 2;
                    while (true)
                    {
                        var newOutputPath = Path.Combine(inputFileInfo.Directory.FullName,
                            inputFileInfo.Name + "_unpacked (" + appendix + ")");

                        if (!Directory.Exists(newOutputPath))
                        {
                            Directory.CreateDirectory(newOutputPath);
                            OutputPath = newOutputPath;
                            break;
                        }
                        appendix++;
                    }
                }
            }

            // TODO 2: Add selective deselection via UI
            var u = new KISSUnpacker();

            try
            {
                if (Register)
                    u.RegisterDefaultShellHandler();
                else if (Unregister)
                    u.UnregisterDefaultShellHandler();
            }
            catch (UnauthorizedAccessException e)
            {
                if (Register)
                    Console.WriteLine("Error: UnauthorizedAccessException. Cannot register explorer context menu handler!");
                if (Unregister)
                    Console.WriteLine("Error: UnauthorizedAccessException. Cannot register explorer context menu handler!");
            }
            if (InputFile != null)
                u.Unpack(InputFile, OutputPath);
        }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
