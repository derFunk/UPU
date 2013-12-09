using System;
using System.Threading.Tasks;
using UpuCore.Config;
using UpuCore.ProcessingSteps.DataContainer;

namespace UpuCore.ProcessingSteps
{
    class FileScanner
    {
        private ILogger _logger;
        private string _inputDir;
        private string _outputDir;

        public FileScanner(string inputDir, string outputDir, ILogger logger)
        {
            _logger = logger;
            _inputDir = inputDir;
            _outputDir = outputDir;
        }


        public async Task<IScannedFiles> StartAsync()
        {
            return null;
        }
    }
}
