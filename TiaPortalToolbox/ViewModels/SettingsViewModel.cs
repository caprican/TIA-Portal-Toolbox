using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.ViewModels;

using TiaPortalToolbox.Models;

namespace TiaPortalToolbox.ViewModels;

// TODO: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
public class SettingsViewModel(IOptions<AppConfig> appConfig, IThemeSelectorService themeSelectorService
                              ,ISystemService systemService, IApplicationInfoService applicationInfoService) : ObservableObject, INavigationAware
{
    private readonly AppConfig appConfig = appConfig.Value;
    private readonly IThemeSelectorService themeSelectorService = themeSelectorService;
    private readonly ISystemService systemService = systemService;
    private readonly IApplicationInfoService applicationInfoService = applicationInfoService;
    private AppTheme theme;
    private string? versionDescription;
    private ICommand? setThemeCommand;
    private ICommand? privacyStatementCommand;

    public AppTheme Theme
    {
        get { return theme; }
        set { SetProperty(ref theme, value); }
    }

    public string VersionDescription
    {
        get { return versionDescription; }
        set { SetProperty(ref versionDescription, value); }
    }

    public ICommand SetThemeCommand => setThemeCommand ??= new RelayCommand<string>(OnSetTheme);

    public ICommand PrivacyStatementCommand => privacyStatementCommand ??= new RelayCommand(OnPrivacyStatement);

    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {applicationInfoService.GetVersion()}";
        Theme = themeSelectorService.GetCurrentTheme();
    }

    public void OnNavigatedFrom()
    {
    }

    public void LoadCompleted()
    {
    }

    private void OnSetTheme(string themeName)
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        themeSelectorService.SetTheme(theme);
    }

    private void OnPrivacyStatement() => systemService.OpenInWebBrowser(appConfig.PrivacyStatement);
}
