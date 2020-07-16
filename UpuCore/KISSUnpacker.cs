using System.Linq;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace UpuCore
{
    public class KISSUnpacker
    {
        /// <summary>
        /// Just do it
        /// </summary>
        /// <param name="inputFilepath">The input filepath.</param>
        /// <param name="outputPath">The output path. Will be generated if it does not exist.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.ArgumentException">File should have unitypackage extension</exception>
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

            if (!inputFilepath.ToLower().EndsWith(".unitypackage"))
                throw new ArgumentException("File should have unitypackage extension");

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            // @see https://github.com/icsharpcode/SharpZipLib/wiki/Code-Reference
            // @see http://msdn.microsoft.com/en-us/library/ms404280(v=vs.110).aspx

            string tempPath = Path.Combine(Path.GetTempPath(), "Upu");
            string f = DecompressGZip(new FileInfo(inputFilepath), tempPath);

            string tempContentPath = Path.Combine(tempPath, "content");
            
            ExtractTar(f, tempContentPath);

            RemapFiles(tempContentPath, outputPath);

            // remove all extracted tem files
            RemoveTempFiles(tempPath);
        }

        /// <summary>
        /// Removes the temporary files.
        /// </summary>
        /// <param name="tempPath">The temporary path.</param>
        private void RemoveTempFiles(string tempPath)
        {
            var tempPathInfo = new DirectoryInfo(tempPath);

            foreach (FileInfo file in tempPathInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in tempPathInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Remaps the files to the remapPath according to the information found in the unityPackage file.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="remapPath">The remapped path.</param>
        private void RemapFiles(string contentPath, string remapPath)
        {
            foreach (var directoryInfo in new DirectoryInfo(contentPath).GetDirectories())
            {
                string pathnameFromFile = File.ReadLines(Path.Combine(directoryInfo.FullName, "pathname")).First();
                pathnameFromFile = pathnameFromFile.Replace('/', Path.DirectorySeparatorChar);

                string assetFilePath = Path.Combine(directoryInfo.FullName, "asset");

                var targetFilePath = Path.Combine(remapPath, pathnameFromFile);
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

        /// <summary>
        /// Decompresses the gzipped file and output it to outputPath.
        /// </summary>
        /// <param name="fileToDecompress">The file to decompress.</param>
        /// <param name="outputPath">The output path.</param>
        /// <returns></returns>
        private string DecompressGZip(FileInfo fileToDecompress, string outputPath)
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
                    using (
                        GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// @See https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#wiki-anchorTar
        /// </summary>
        /// <param name="tarFileName"></param>
        /// <param name="destFolder"></param>
        public void ExtractTar(String tarFileName, String destFolder)
        {
            Console.WriteLine("Extracting " + tarFileName + " to " + destFolder + "...");
            
            // We have to leave the unpack-directory in order to be able to delete the temp
            // files afterwards again, especially on Windows.
            string formerDir = Directory.GetCurrentDirectory();

            using (Stream inStream = File.OpenRead(tarFileName))
            {
                using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream))
                {
                    Directory.CreateDirectory(destFolder);
                    Directory.SetCurrentDirectory(destFolder);
                    tarArchive.ExtractContents(".");
                }
            }

            Directory.SetCurrentDirectory(formerDir);
        }

        /// <summary>
        /// Registers the default shell handler.
        /// </summary>
        public void RegisterDefaultShellHandler()
        {
            // http://www.codeproject.com/Articles/15171/Simple-shell-context-menu
            // get full path to self, %L is a placeholder for the selected file
            string menuCommand = string.Format("\"{0}\" -i \"%L\"", Assembly.GetEntryAssembly().Location);
            RegisterShellHandler("Unity package file", "Unpack", "Unpack here", menuCommand);
        }

        /// <summary>
        /// Unregisters the default shell handler.
        /// </summary>
        public void UnregisterDefaultShellHandler()
        {
            // sample usage to unregister
            UnregisterShellHandler("Unity package file", "Unpack");
        }

        /// <summary>
        /// Registers the shell handler.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <param name="shellKeyName">Name of the shell key.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="menuCommand">The menu command.</param>
        private void RegisterShellHandler(string fileType,
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

        /// <summary>
        /// Unregisters the shell handler.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <param name="shellKeyName">Name of the shell key.</param>
        private void UnregisterShellHandler(string fileType, string shellKeyName)
        {
            if (string.IsNullOrEmpty(fileType) || string.IsNullOrEmpty(shellKeyName))
                return;

            // path to the registry location
            string regPath = string.Format(@"{0}\shell\{1}",
                fileType, shellKeyName);

            // remove context menu from the registry
            Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }
    }
}