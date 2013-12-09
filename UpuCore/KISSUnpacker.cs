using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace UpuCore
{
    public class KISSUnpacker
    {
        /// <summary>
        /// Just do it
        /// </summary>
        public void Unpack(string inputFilepath, string outputPath)
        {
            Console.WriteLine("Extracting " + inputFilepath + " to " + outputPath);

            // test absolute
            if (!File.Exists(inputFilepath))
            {
                // test relative
                inputFilepath = Path.Combine(Environment.CurrentDirectory, inputFilepath);
                if (!File.Exists(inputFilepath))
                    throw new FileNotFoundException(inputFilepath);
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (!inputFilepath.EndsWith(".unitypackage"))
                throw new ArgumentException("File should have unitypackage extension");

            // @see https://github.com/icsharpcode/SharpZipLib/wiki/Code-Reference
            // @see http://msdn.microsoft.com/en-us/library/ms404280(v=vs.110).aspx

            string tempPath = Path.Combine(Path.GetTempPath(), "Upu");
            string f = DecompressGZip(new FileInfo(inputFilepath), tempPath);

            string contentPath = Path.Combine(Path.GetTempPath(), "Upu\\content");
            ExtractTar(f, contentPath);

            RemapFiles(contentPath, outputPath);

            // remove all extracted tem files
            RemoveTempFiles(tempPath);
        }

        private void RemoveTempFiles(string tempPath)
        {
            DirectoryInfo downloadedMessageInfo = new DirectoryInfo(tempPath);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void RemapFiles(string contentPath, string remappedPath)
        {
            foreach (var directoryInfo in new DirectoryInfo(contentPath).GetDirectories())
            {
                string pathname = File.ReadLines(Path.Combine(directoryInfo.FullName, "pathname")).First();
                pathname = pathname.Replace('/', Path.DirectorySeparatorChar);

                string assetFilePath = Path.Combine(directoryInfo.FullName, "asset");

                var targetFilePath = Path.Combine(remappedPath, pathname);
                var targetPath = new FileInfo(targetFilePath).Directory.FullName;

                if (!Directory.Exists(targetPath))
                {
                    Console.WriteLine("Creating directory " + targetPath + "...");
                    Directory.CreateDirectory(targetPath);
                }

                if (File.Exists(assetFilePath) && !File.Exists(targetFilePath))
                {
                    Console.WriteLine("Extracting file " + targetFilePath + "...");
                    File.Move(assetFilePath, targetFilePath);
                }
            }
        }

        public static string DecompressGZip(FileInfo fileToDecompress, string outputPath)
        {
            string target;

            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string newFileName = fileToDecompress.Name;
                if (fileToDecompress.Extension.Length > 0)
                    newFileName = newFileName.Remove(newFileName.Length - fileToDecompress.Extension.Length);

                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

                target = Path.Combine(outputPath, newFileName);

                using (FileStream decompressedFileStream = File.Create(target))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#wiki-anchorTar
        /// </summary>
        /// <param name="tarFileName"></param>
        /// <param name="destFolder"></param>
        public void ExtractTar(String tarFileName, String destFolder)
        {
            using(Stream inStream = File.OpenRead(tarFileName))
            {
                TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
                tarArchive.ExtractContents(destFolder);
                tarArchive.Close();
            }
    }

        public void RegisterHandler()
        {
            // http://www.codeproject.com/Articles/15171/Simple-shell-context-menu
            // get full path to self, %L is a placeholder for the selected file
            string menuCommand = string.Format("\"{0}\" \"%L\"", Assembly.GetEntryAssembly().Location);
            Register("Unity package file", "Unpack", "Unpack here", menuCommand);
        }

        public void UnregisterHandler()
        {
            // sample usage to unregister
            Unregister("Unity package file", "Unpack");
        }

        private void Register(string fileType,
           string shellKeyName, string menuText, string menuCommand)
        {
            // create path to registry location
            string regPath = string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key =
                   Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }

        private void Unregister(string fileType, string shellKeyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileType) &&
                !string.IsNullOrEmpty(shellKeyName));

            // path to the registry location
            string regPath = string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);

            // remove context menu from the registry
            Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }
    }
}
