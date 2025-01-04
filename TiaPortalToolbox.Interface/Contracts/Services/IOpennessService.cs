namespace TiaPortalToolbox.Core.Contrats.Services;

public interface IOpennessService
{
    public List<string>? EngineeringVersions { get; }

    internal void Initialize();
}
