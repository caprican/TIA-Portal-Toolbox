using System.IO;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using TiaPortalOpenness.Contracts.Services;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Properties;

namespace TiaPortalToolbox.ViewModels;

public class NoProjectViewModel(INavigationService navigationService, IDialogCoordinator dialogCoordinator, 
                                IOpennessService opennessService) : ObservableObject, INavigationAware
{
    private readonly INavigationService navigationService = navigationService;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;

    private ICommand? openCommand;
    private ICommand? attachCommand;

    public ICommand OpenCommand => openCommand ??= new AsyncRelayCommand(OnOpen);
    public ICommand AttachCommand => attachCommand ??= new AsyncRelayCommand(OnAttach);

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {

    }

    public void LoadCompleted()
    {
    }

    private async Task OnOpen()
    {
        var version = opennessService.EngineeringVersion?.Major.ToString();
        if (opennessService.EngineeringVersion?.Minor > 0)
        {
            version += $"_{opennessService.EngineeringVersion.Minor}";
        }

        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Filter = $"Projets TIA Portal | *.ap{version};*.als{version};*.amc{version}",
            Title = Resources.OpenProjectDialogTitle,
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Automation")
        };

        if (dialog.ShowDialog() == true)
        {
            var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Resources.OpenProjectProgressDialogTitle, Resources.OpenProjectProgressDialogStarting);
            progress.SetIndeterminate();

            opennessService.ProjectOpenning += (sender, e) => progress.SetMessage($"Openning project {e}.");

            await opennessService.OpenOrAttachProjectAsync(path: dialog.FileName);
            await progress.CloseAsync();
            navigationService.NavigateTo(typeof(ViewModels.MainViewModel).FullName, clearNavigation:true);
        }
    }

    private async Task OnAttach()
    {
        var customDialog = new CustomDialog { Title = Resources.SelectProjectDialogTitle };
        var dataContext = new Dialogs.SelectProjectViewModel(async instance =>
        {
            await dialogCoordinator.HideMetroDialogAsync(App.Current.MainWindow.DataContext, customDialog);
        }, async instance =>
        {
            await dialogCoordinator.HideMetroDialogAsync(App.Current.MainWindow.DataContext, customDialog);

            var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Resources.OpenProjectProgressDialogTitle, Resources.OpenProjectProgressDialogAttaching);
            progress.SetIndeterminate();
            await opennessService.OpenOrAttachProjectAsync(instance.SelectedProcess);
            await progress.CloseAsync();

            navigationService.NavigateTo(typeof(ViewModels.MainViewModel).FullName, clearNavigation:true);
        }, opennessService);
        customDialog.Content = new Views.Dialogs.SelectProjectDialog { DataContext = dataContext };

        await dialogCoordinator.ShowMetroDialogAsync(App.Current.MainWindow.DataContext, customDialog);
    }
}
