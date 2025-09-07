using TiaPortalOpenness.Models.ProjectTree;

namespace TiaPortalOpenness.Contracts.Services;

public interface IUnifiedService
{
    public Task BuildHmiTags(IEnumerable<Connexion> connexions, Action<string> setMessage, string? defaultAlarmsClass);
    public Task BuildHmiAlarms(IEnumerable<Connexion> connexions, Action<string> setMessage, string? defaultAlarmsClass);
    public Task GetTagAlarms(IEnumerable<Connexion> connexions, Action<string> setMessage, string? defaultAlarmsClass);
}
