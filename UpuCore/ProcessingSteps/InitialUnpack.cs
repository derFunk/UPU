using System;
using System.IO;
using System.Threading.Tasks;
using UpuCore.Config;

namespace UpuCore.ProcessingSteps
{
    class InitialUnpack : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _unityFile;
        private readonly string _outputDir;

        public InitialUnpack(string unityFile, string outputDir, ILogger logger)
        {
            _logger = logger;
            _unityFile = unityFile;
            _outputDir = outputDir;
        }

        public async Task StartAsync()
        {
            
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_outputDir, true);
            }
            catch (Exception e)
            {
                _logger.LogError("Error when deleting output Directory of InitialUnpack {0}: {1}, Trace: {2}", _outputDir, e.Message, e.StackTrace);
            }
        }
    }
}
