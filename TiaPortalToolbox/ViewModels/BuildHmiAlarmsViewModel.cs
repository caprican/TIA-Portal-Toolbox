using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Core.Services;
using TiaPortalToolbox.Models;
using TiaPortalToolbox.Properties;

namespace TiaPortalToolbox.ViewModels;

public class BuildHmiAlarmsViewModel(IDialogCoordinator dialogCoordinator, IOpennessService opennessService, IPlcService plcService, IHmiService hmiService, IUnifiedService unifiedService) : ObservableObject, INavigationAware
{
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;

    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly IHmiService hmiService = hmiService;
    private readonly IUnifiedService unifiedService = unifiedService;

    private ICommand? refreshListCommand;
    private ICommand? buildAlarmsCommand;
    private ICommand? buildTagsCommand;

    private ObservableCollection<Connexion>? connexions;
    private Connexion? connexionSelected;
    private bool simplifyTagname = false;
    private string dataBlockMark = "_Defauts";

    public ICommand BuildAlarmsCommand => buildAlarmsCommand ??= new AsyncRelayCommand(OnBuildAlarms);
    public ICommand BuildTagsCommand => buildTagsCommand ??= new AsyncRelayCommand(OnBuildTags);
    public ICommand RefreshListCommand => refreshListCommand ??= new AsyncRelayCommand(OnRefreshList);

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
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
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
        var defaultAlarmsClass = "Alarm";

        //Task.Run(() => { plcProjectService.BuildHmiTags(Devices.Where(w => w.Selected == true).ToList(), defaultAlarmsClass, SimplifyTagname); });
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();

        await unifiedService.BuildHmiTags(Connexions.Where(w => w.Selected == true).Select(s => s.connexion), defaultAlarmsClass, false);

        await progress.CloseAsync();
    }

    private async Task OnBuildAlarms()
    {
        var defaultAlarmsClass = "Alarm";

        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();

        await unifiedService.BuildHmiAlarms(Connexions.Where(w => w.Selected == true).Select(s => s.connexion), defaultAlarmsClass, false);
        
        await progress.CloseAsync();
    }
}
