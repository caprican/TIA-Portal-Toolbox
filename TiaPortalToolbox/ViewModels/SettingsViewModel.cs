using System.Globalization;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.ViewModels;

using TiaPortalToolbox.Models;
using TiaPortalToolbox.Services;

namespace TiaPortalToolbox.ViewModels;

public class SettingsViewModel(IOptions<AppConfig> appConfig, IThemeSelectorService themeSelectorService, ISettingsService settingsService
                              , ISystemService systemService, IApplicationInfoService applicationInfoService) : ObservableObject, INavigationAware
{
    private readonly AppConfig appConfig = appConfig.Value;
    private readonly IThemeSelectorService themeSelectorService = themeSelectorService;
    private readonly ISystemService systemService = systemService;
    private readonly IApplicationInfoService applicationInfoService = applicationInfoService;
    private AppTheme theme;
    private string? versionDescription;
    private Core.Models.LanguageItem? language;

    private ICommand? setThemeCommand;
    private ICommand? privacyStatementCommand;

    public AppTheme Theme
    {
        get => theme;
        set { SetProperty(ref theme, value); }
    }

    public string? VersionDescription
    {
        get => versionDescription;
        set { SetProperty(ref versionDescription, value); }
    }

    public Core.Models.LanguageItem[] Languages
    {
        get =>
        [
            new Core.Models.LanguageItem{ Name = "English", Culture = "en-US" },
            new Core.Models.LanguageItem{ Name = "Deutsch", Culture = "de-DE" },
            new Core.Models.LanguageItem{ Name = "Français", Culture = "fr-FR" },
            new Core.Models.LanguageItem{ Name = "Español", Culture = "es-ES" },
            new Core.Models.LanguageItem{ Name = "Italiano", Culture = "it-IT" },
            //new Core.Models.LanguageItem{ Name = "Português", Culture = "de-DE" },
            //new Core.Models.LanguageItem{ Name = "рyсский", Culture = "de-DE" },
            //new Core.Models.LanguageItem{ Name = "日本語", Culture = "de-DE" },
            //new Core.Models.LanguageItem{ Name = "한국어", Culture = "de-DE" },
        ];
    }

    public Core.Models.LanguageItem? Language
    {
        get => language;
        set
        {
            SetProperty(ref language, value);
            var info = CultureInfo.CreateSpecificCulture(Language?.Culture ?? "en-US");
            App.Current.Properties[nameof(settingsService.CurrentCulture)] = info.Name;
            Thread.CurrentThread.CurrentCulture = info;
            Thread.CurrentThread.CurrentUICulture = info;

            //App.ChangeCulture(language.Culture);
        }
    }

    public ICommand SetThemeCommand => setThemeCommand ??= new RelayCommand<string>(OnSetTheme);

    public ICommand PrivacyStatementCommand => privacyStatementCommand ??= new RelayCommand(OnPrivacyStatement);

    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {applicationInfoService.GetVersion()}";
        Theme = themeSelectorService.GetCurrentTheme();
        language = Languages.SingleOrDefault(s => s.Culture == settingsService.CurrentCulture);

    }

    public void OnNavigatedFrom()
    {
    }

    public void LoadCompleted()
    {
    }

    private void OnSetTheme(string? themeName)
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        themeSelectorService.SetTheme(theme);
    }

    private void OnPrivacyStatement() => systemService.OpenInWebBrowser(appConfig.PrivacyStatement);
}
