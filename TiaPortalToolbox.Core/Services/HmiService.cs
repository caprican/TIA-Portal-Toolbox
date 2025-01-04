using TiaPortalToolbox.Core.Contracts.Services;

namespace TiaPortalToolbox.Core.Services;

public class HmiService(IOpennessService opennessService) : IHmiService
{
    private readonly IOpennessService opennessService = opennessService;

}
