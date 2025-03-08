using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiaPortalToolbox.Contracts.Services;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;

namespace TiaPortalToolbox.ViewModels;

public class MainViewModel(INavigationService navigationService, IOpennessService opennessService, 
                            IShellWindow shellWindow) : ObservableObject, INavigationAware
{
    private readonly INavigationService navigationService = navigationService;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IShellWindow shellWindow = shellWindow;

    private ICommand? gotoBuildHmiAlarmsCommand;
    private ICommand? gotoDocumensBuilderCommand;

    public ICommand GotoBuildHmiAlarmsCommand => gotoBuildHmiAlarmsCommand ??= new RelayCommand(() => navigationService.NavigateTo(typeof(BuildHmiAlarmsViewModel).FullName));
    public ICommand GotoDocumensBuilderCommand => gotoDocumensBuilderCommand ??= new RelayCommand(() => navigationService.NavigateTo(typeof(DocumentViewModel).FullName, shellWindow.SelectedItem));

    public void OnNavigatedTo(object parameter)
    {
        //plcProjectService.ProjectUpdate += PlcProjectService_ProjectUpdate;
    }

    public void OnNavigatedFrom()
    {
        //plcProjectService.ProjectUpdate -= PlcProjectService_ProjectUpdate;
    }

    private void PlcProjectService_ProjectUpdate(object sender, EventArgs e)
    {
    }

    public void LoadCompleted()
    {
    }
}
