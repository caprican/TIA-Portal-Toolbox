using TiaPortalToolbox.Core.Models.ProjectTree;

namespace TiaPortalToolbox.Core.Contracts.Services;

public interface IUnifiedService
{
    public Task BuildHmiTags(IEnumerable<Connexion> connexions, string defaultAlarmsClass, bool simplifyTagname);
}
