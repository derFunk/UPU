using System.Collections.Generic;

namespace UpuCore.ProcessingSteps.DataContainer.Impl
{
    class ScannedFiles : IScannedFiles
    {
        public List<string> Files { get; private set; }
        public List<string> Directories { get; private set; }
    }
}
