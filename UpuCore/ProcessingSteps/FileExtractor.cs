using System;
using System.Threading.Tasks;
using UpuCore.Config;
using UpuCore.ProcessingSteps.DataContainer;

namespace UpuCore.ProcessingSteps
{
    class FileExtractor : IDisposable
    {
        private bool _success;
        private ILogger _logger;
        private string _inputDir;
        private IScannedFiles _scannedFiles;

        public FileExtractor(string inputDir, IScannedFiles scannedFiles, ILogger logger)
        {
            _logger = logger;
            _inputDir = inputDir;
            _scannedFiles = scannedFiles;
        }

        public async Task StartAsync()
        {
            
        }

        public void Dispose()
        {
            if (!_success)
                CleanUp();
        }

        private void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}
