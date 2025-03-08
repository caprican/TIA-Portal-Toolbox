using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Core.Models.ProjectTree;
using TiaPortalToolbox.Properties;

namespace TiaPortalToolbox.ViewModels;

public partial class ShellViewModel(INavigationService navigationService, IDialogCoordinator dialogCoordinator,
                                    IOpennessService opennessService, ISettingsService settingsService) : ObservableObject
{
    private readonly INavigationService navigationService = navigationService;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;
    private readonly ISettingsService settingsService = settingsService;

    private ICommand? goBackCommand;
    private ICommand? loadedCommand;
    private ICommand? unloadedCommand;
    private ICommand? exportItemCommand;
    private ICommand? buildDocumentsCommand;
    private bool paneOpen = false;
    private ObservableCollection<Core.Models.ProjectTree.Object>? projectTreeItems;

    public ICommand GoBackCommand => goBackCommand ??= new RelayCommand(OnGoBack, CanGoBack);

    public ICommand LoadedCommand => loadedCommand ??= new AsyncRelayCommand(OnLoaded);
    public ICommand UnloadedCommand => unloadedCommand ??= new RelayCommand(OnUnloaded);

    public ICommand ExportItemCommand => exportItemCommand ??= new AsyncRelayCommand<Item>(OnExport);
    public ICommand BuildDocumentsCommand => buildDocumentsCommand ??= new AsyncRelayCommand<Core.Models.ProjectTree.Object?>(OnBuildDocuments);

    public bool PaneOpen
    {
        get => paneOpen;
        set => SetProperty(ref paneOpen, value);
    }

    public ObservableCollection<Core.Models.ProjectTree.Object>? ProjectTreeItems
    {
        get => projectTreeItems;
        set => SetProperty(ref projectTreeItems, value);
    }

    private async Task OnLoaded()
    {
        opennessService.NewProjectOpenned += OnNewProjectOpenned;

        var customDialog = new CustomDialog { Title = Resources.PreSelectionAssemblyTitle };
        var dataContext = new Dialogs.PreSelectionAssemblyVersionViewModel (async instance =>
        {
            await dialogCoordinator.HideMetroDialogAsync(App.Current.MainWindow.DataContext, customDialog);

            opennessService.Initialize(instance.EngineeringVersion, instance.OpennessApiVersion);
            navigationService.NavigateTo(typeof(ViewModels.NoProjectViewModel).FullName);
        }, opennessService, settingsService);
        customDialog.Content = new Views.Dialogs.PreSelectionAssemblyVersionView { DataContext = dataContext };

        await dialogCoordinator.ShowMetroDialogAsync(App.Current.MainWindow.DataContext, customDialog);
    }

    private void OnUnloaded()
    {
        PaneOpen = false;
        opennessService.NewProjectOpenned -= OnNewProjectOpenned;
    }

    private bool CanGoBack() => navigationService.CanGoBack;

    private void OnGoBack() => navigationService.GoBack();

    private void OnNewProjectOpenned(object sender, EventArgs e)
    {
        PaneOpen = true;

        ProjectTreeItems = opennessService.ProjectTreeItems;
    }

    private async Task OnExport(Item? item)
    {
        if (item is null) return;

        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();

        await opennessService.ExportAsync(item);
        
        await progress.CloseAsync();
    }

    private async Task OnBuildDocuments(Core.Models.ProjectTree.Object? items)
    {
        if(items is null) return;


        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();
        





        await progress.CloseAsync();
    }
}
