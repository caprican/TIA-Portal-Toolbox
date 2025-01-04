using TiaPortalToolbox.Contracts.Services;

namespace TiaPortalToolbox.Services;

public class SettingsService : ISettingsService
{
    public string? Theme => App.Current.Properties[nameof(Theme)]?.ToString();
    public string? CurrentCulture => App.Current.Properties[nameof(CurrentCulture)]?.ToString();

    public string? SelectedEngineeringVersion => App.Current.Properties[nameof(SelectedEngineeringVersion)]?.ToString();

    public string? SelectedOpennessApiVersion => App.Current.Properties[nameof(SelectedOpennessApiVersion)]?.ToString();

    public bool HidePreSelectionAssemblyVersion => (bool)App.Current.Properties[nameof(HidePreSelectionAssemblyVersion)];

    public void InitializeSettings()
    {
        if (!App.Current.Properties.Contains(nameof(SelectedEngineeringVersion)) || string.IsNullOrEmpty(App.Current.Properties[nameof(SelectedEngineeringVersion)]?.ToString()))
        {
            App.Current.Properties[nameof(SelectedEngineeringVersion)] = string.Empty;
        }
        if (!App.Current.Properties.Contains(nameof(SelectedOpennessApiVersion)) || string.IsNullOrEmpty(App.Current.Properties[nameof(SelectedOpennessApiVersion)]?.ToString()))
        {
            App.Current.Properties[nameof(SelectedOpennessApiVersion)] = string.Empty;
        }
        if (!App.Current.Properties.Contains(nameof(HidePreSelectionAssemblyVersion)) || string.IsNullOrEmpty(App.Current.Properties[nameof(HidePreSelectionAssemblyVersion)]?.ToString()))
        {
            App.Current.Properties[nameof(HidePreSelectionAssemblyVersion)] = false;
        }
    }

    public void SaveSelectAssemblyVersion(string engineeringVersion, string opennessVersion, bool remember)
    {
        App.Current.Properties[nameof(SelectedEngineeringVersion)] = engineeringVersion;
        App.Current.Properties[nameof(SelectedOpennessApiVersion)] = opennessVersion;
        App.Current.Properties[nameof(HidePreSelectionAssemblyVersion)] = remember;
    }
}