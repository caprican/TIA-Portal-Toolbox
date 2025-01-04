using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;

namespace TiaPortalToolbox.ViewModels;

public class DocumentViewModel(IShellWindow shellWindow, IDialogCoordinator dialogCoordinator, IOpennessService opennessService,
                               IPlcService plcService) : ObservableObject, INavigationAware
{
    private readonly IShellWindow shellWindow = shellWindow;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;

    private List<CultureInfo>? projectLangages;
    private CultureInfo? referenceLanguage;
    private CultureInfo? editingLanguage;

    private Core.Models.ProjectTree.Plc.Blocks.Object? plcBlock;

    public List<CultureInfo>? ProjectLangages
    {
        get => projectLangages;
        set => SetProperty(ref projectLangages, value);
    }

    public CultureInfo? ReferenceLanguage
    {
        get => referenceLanguage;
        set
        {
            SetProperty(ref referenceLanguage, value);
        }
    }

    public CultureInfo? EditingLanguage
    {
        get => editingLanguage;
        set
        {
            SetProperty(ref editingLanguage, value);
        }
    }

    public Core.Models.ProjectTree.Plc.Blocks.Object? PlcBlock
    {
        get => plcBlock;
        set => SetProperty(ref plcBlock, value);
    }

    public void OnNavigatedFrom()
    {
        shellWindow.SelectedItemChanged -= OnSelectedItemChanged;
    }

    public void OnNavigatedTo(object parameter)
    {
        shellWindow.SelectedItemChanged += OnSelectedItemChanged;

        ProjectLangages = opennessService.ProjectLanguages;
        EditingLanguage = opennessService.EditingLanguage;
        ReferenceLanguage = opennessService.ReferenceLanguage;
    }

    private void OnSelectedItemChanged(object sender, EventArgs e)
    {
        //plcService.GetMetaDataBlock(ref e);
    }

    public void LoadCompleted()
    {

    }
}
