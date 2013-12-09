using System.Collections.Generic;

namespace UpuCore.ProcessingSteps.DataContainer
{
    interface IScannedFiles
    {
        List<string> Files { get; }
        List<string> Directories { get; } 
    }
}
