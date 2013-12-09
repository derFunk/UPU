using System.Threading.Tasks;
using UpuCore.Config;
using UpuCore.ProcessingSteps;

namespace UpuCore
{
    public static class Unpacker
    {
        public static async Task Unpack(UnpackerConfig config)
        {
            using (var initialUnpack = new InitialUnpack(config.InputUnityFile, config.WorkingDir, config.Logger))
            {
                await initialUnpack.StartAsync();
                var fileScanner = new FileScanner(config.WorkingDir, config.OutputDir, config.Logger);
                var scannedFiles = await fileScanner.StartAsync();

                using (var fileExtractor = new FileExtractor(config.WorkingDir, scannedFiles, config.Logger))
                {
                    await fileExtractor.StartAsync();
                }
            }
        }
    }
}
