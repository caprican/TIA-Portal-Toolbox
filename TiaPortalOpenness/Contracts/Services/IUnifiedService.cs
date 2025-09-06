using TiaPortalOpenness.Models.ProjectTree;

namespace TiaPortalOpenness.Contracts.Services;

public interface IUnifiedService
{
    public Task BuildHmiTags(IEnumerable<Connexion> connexions, string? defaultAlarmsClass, bool simplifyTagname);
    public Task BuildHmiAlarms(IEnumerable<Connexion> connexions, string? defaultAlarmsClass, bool simplifyTagname);
}
