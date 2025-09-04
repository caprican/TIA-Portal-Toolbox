using TiaPortalOpenness.Contracts.Services;

namespace TiaPortalOpenness.Services;

public class HmiService(IOpennessService opennessService) : IHmiService
{
    private readonly IOpennessService opennessService = opennessService;

}
