namespace TiaPortalToolbox.Contracts.Services;

public interface ISettingsService
{
    public string? Theme { get; }
    public string? CurrentCulture { get; }
    public string? SelectedEngineeringVersion { get; }
    public string? SelectedOpennessApiVersion { get; }
    public bool HidePreSelectionAssemblyVersion { get; }

    void InitializeSettings();
    void SaveSelectAssemblyVersion(string engineeringVersion, string opennessVersion, bool remember);
}
