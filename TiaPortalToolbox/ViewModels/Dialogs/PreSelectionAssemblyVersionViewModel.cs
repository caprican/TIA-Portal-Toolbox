using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TiaPortalToolbox.Contracts.Services;

namespace TiaPortalToolbox.ViewModels.Dialogs;

public partial class PreSelectionAssemblyVersionViewModel(Action<PreSelectionAssemblyVersionViewModel> closeHandler,
                                                          Core.Contracts.Services.IOpennessService opennessService, ISettingsService settingsService) : ObservableObject
{
    private readonly Core.Contracts.Services.IOpennessService opennessService = opennessService;
    private readonly ISettingsService settingsService = settingsService;

    private ICommand? closeCommand;
    private List<string>? engineeringVersions;
    private List<string>? opennessApiVersions;
    private string? engineeringVersion;
    private string? opennessApiVersion;

    public ICommand CloseCommand => closeCommand ??= new RelayCommand(() => closeHandler(this));

    /// <summary>
    /// Indicates the visibility of the dialog.
    /// That means, will be the dialog, on start of the demo application, displayed or not.
    /// </summary>
    public bool HidePreSelectionAssemblyVersionDialog { get; set; } = settingsService.HidePreSelectionAssemblyVersion;

    /// <summary>
    /// Installed versions of the engineering dll
    /// </summary>
    public List<string>? EngineeringVersions
    {
        get => opennessService.EngineeringVersions;
        set => SetProperty(ref engineeringVersions, value);
    }

    /// <summary>
    /// Installed versions of the openness api 
    /// </summary>
    public List<string>? OpennessApiVersions
    {
        get => opennessApiVersions;
        set => SetProperty(ref opennessApiVersions, value);
    }

    /// <summary>
    /// Selected version of the engineering dll
    /// </summary>
    public string? EngineeringVersion
    {
        get => engineeringVersion;
        set
        {
            SetProperty(ref engineeringVersion, value);
            if (!string.IsNullOrEmpty(engineeringVersion))
            {
                if (opennessService.GetOpennessVersions(engineeringVersion) is List<string> opennessVersions)
                {
                    OpennessApiVersions = opennessVersions;
                    OpennessApiVersion = opennessVersions.Last();
                }
                else
                {
                    OpennessApiVersions = [];
                    OpennessApiVersion = null;
                }
            }
        }
    }

    /// <summary>
    /// Selected version of the openness api
    /// </summary>
    public string? OpennessApiVersion
    {
        get => opennessApiVersion;
        set => SetProperty(ref opennessApiVersion, value);
    }

    private void OnLoaded()
    {
        //EngineeringVersions = tiaPortalService.EngineeringVersions;

        //foreach (var version in EngineeringVersions.OrderByDescending(o => o))
        //{
        //    if (tiaPortalService.GetOpennessVersions(version) is List<string> apiVersion)
        //    {
        //        OpennessApiVersions = apiVersion;
        //        EngineeringVersion = version;
        //        OpennessApiVersion = apiVersion.Last();
        //        break;
        //    }
        //}
    }

    private void GetOpennessApiVersions(string? version)
    {
        //if (tiaPortalService.GetOpennessVersions(version) is List<string> apiVersion)
        //{
        //    OpennessApiVersions = apiVersion;
        //    OpennessApiVersion = apiVersion.Last();
        //}
        //else
        //{
        //    OpennessApiVersions.Clear();
        //    OpennessApiVersion = null;
        //}
    }


}
