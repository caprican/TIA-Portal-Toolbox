using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW;

using TiaPortalToolbox.Core.Contracts.Services;
using Siemens.Engineering;
using System.IO;

namespace TiaPortalToolbox.Core.Services;

public class PlcService(IOpennessService opennessService) : IPlcService
{
    private readonly IOpennessService opennessService = opennessService;
    
    public void GetMetaDataBlock(ref Models.ProjectTree.Object plcItem)
    {

    }
}
