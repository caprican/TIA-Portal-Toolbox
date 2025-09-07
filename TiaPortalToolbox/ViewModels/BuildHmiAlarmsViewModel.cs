using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Options;
using Microsoft.Win32;

using TiaPortalOpenness.Contracts.Services;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Models;
using TiaPortalToolbox.Properties;
using TiaPortalToolbox.Table.Contracts.Builders;

namespace TiaPortalToolbox.ViewModels;

public class BuildHmiAlarmsViewModel(IDialogCoordinator dialogCoordinator, IOpennessService opennessService, IPlcService plcService, IHmiService hmiService, IUnifiedService unifiedService
                                    , IOptions<Table.Models.SpreadsheetSettings> settings, ISpreadsheetBuilder spreadsheetBuilder) : ObservableObject, INavigationAware
{
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;

    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly IHmiService hmiService = hmiService;
    private readonly IUnifiedService unifiedService = unifiedService;

    private readonly Table.Models.SpreadsheetSettings? settings = settings?.Value;
    private readonly ISpreadsheetBuilder spreadsheetBuilder = spreadsheetBuilder;

    private ICommand? refreshListCommand;
    private ICommand? buildAlarmsCommand;
    private ICommand? buildTagsCommand;
    private ICommand? builCommand;

    private ObservableCollection<Connexion>? connexions;
    private Connexion? connexionSelected;
    private bool simplifyTagname = false;
    private string dataBlockMark = "_Defauts";

    public ICommand BuildAlarmsCommand => buildAlarmsCommand ??= new AsyncRelayCommand(OnBuildAlarms);
    public ICommand BuildTagsCommand => buildTagsCommand ??= new AsyncRelayCommand(OnBuildTags);
    public ICommand RefreshListCommand => refreshListCommand ??= new AsyncRelayCommand(OnRefreshList);
    public ICommand BuildCommand => builCommand ??= new AsyncRelayCommand(OnBuild);

    public ObservableCollection<Connexion>? Connexions
    {
        get => connexions;
        set => SetProperty(ref connexions, value);
    }

    public Connexion? ConnexionSelected
    {
        get => connexionSelected;
        set => SetProperty(ref connexionSelected, value);
    }

    public bool SimplifyTagname
    {
        get => simplifyTagname;
        set => SetProperty(ref simplifyTagname, value);
    }

    public string DataBlockMark
    {
        get => dataBlockMark;
        set
        {
            SetProperty(ref dataBlockMark, value);
            RefreshListCommand.Execute(null);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        RefreshListCommand.Execute(null);
    }

    public void LoadCompleted()
    {


    }

    private async Task OnRefreshList()
    {
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Resources.BuildHmiAlarmsPageRefreshTite, Resources.BuildHmiAlarmsPageRefreshText);
        progress.SetIndeterminate();

        Connexions = [];
        foreach(var connexion in await opennessService.GetCommunDataBlock(DataBlockMark))
        {
            Connexions.Add(new Connexion(connexion));
        }

        await progress.CloseAsync();
    }

    private async Task OnBuildTags()
    {
        //var defaultAlarmsClass = "Alarm";

        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Resources.BuildHmiAlarmsPageBuildTagsTitle, Resources.BuildHmiAlarmsPageBuildTagsText);
        progress.SetIndeterminate();

        await unifiedService.BuildHmiTags(Connexions.Where(w => w.Selected == true).Select(s => s.connexion), message => progress.SetMessage(message), /*defaultAlarmsClass*/null);

        await progress.CloseAsync();
    }

    private async Task OnBuildAlarms()
    {
        //var defaultAlarmsClass = "Alarm";

        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Resources.BuildHmiAlarmsPageBuildAlarmsTitle, Resources.BuildHmiAlarmsPageBuildAlarmsText);
        progress.SetIndeterminate();

        await unifiedService.BuildHmiAlarms(Connexions.Where(w => w.Selected == true).Select(s => s.connexion), message => progress.SetMessage(message), /*defaultAlarmsClass*/null);
        
        await progress.CloseAsync();
    }

    private async Task OnBuild()
    {
        if (settings is null)
        {
            await dialogCoordinator.ShowMessageAsync(App.Current.MainWindow.DataContext, Properties.Resources.DocumentPageMessageErrorTitle, Properties.Resources.DocumentPageMessageErrorSettingsText);
            return;
        }

        settings.ProjectPath = opennessService.ProjectPath ?? "";
        settings.UserFolderPath = opennessService.UserFolder ?? "";

        var exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"{opennessService.ProjectName}_HMIAlarms.xlsx");
#if !DEBUG
        var saveFileDialog = new SaveFileDialog()
        {
            Title = "Select the location to save the document",
            FileName = $"{opennessService.ProjectName}_HMIAlarms.xlsx",
            Filter = "Excel spreadsheet (*.xlsx)|*.xlsx",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (saveFileDialog.ShowDialog() != true)
        {
            exportPath = saveFileDialog.FileName;
        }
#endif
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();

        var connexions = Connexions.Where(w => w.Selected == true).Select(s => s.connexion);
        await unifiedService.GetTagAlarms(connexions, message => progress.SetMessage(message), null);

        foreach(var connexion in connexions)
        {
            if(opennessService.ProjectLanguages?.Count > 0)
            {
                foreach (var language in opennessService.ProjectLanguages)
                {
                    settings.SpreadsheetPath = $@"{Path.GetDirectoryName(exportPath)}\{Path.GetFileNameWithoutExtension(exportPath)}_{language.Name}.{Path.GetExtension(exportPath)}";
                    await spreadsheetBuilder.CreateSpreadsheet(connexion, language);
                
                }
            }
        }

        await progress.CloseAsync();
    }
}
