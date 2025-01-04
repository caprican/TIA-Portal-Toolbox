
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TiaPortalToolbox.ViewModels.Dialogs;

public class SelectProjectViewModel(Action<SelectProjectViewModel> closeHandler, Action<SelectProjectViewModel> openHandler,
                                    Core.Contracts.Services.IOpennessService opennessService) : ObservableObject
{
    private readonly Core.Contracts.Services.IOpennessService opennessService = opennessService;

    private ICommand? closeCommand;
    private ICommand? attachCommand;

    private Core.Models.TiaProcess? selectedProcess;

    public ICommand CloseCommand => closeCommand ??= new RelayCommand(() => closeHandler(this));
    public ICommand AttachCommand => attachCommand ??= new RelayCommand(() => openHandler(this));

    public List<Core.Models.TiaProcess> Processes => opennessService.GetProcesses();

    public Core.Models.TiaProcess? SelectedProcess
    {
        get => selectedProcess;
        set => SetProperty(ref selectedProcess, value);
    }
}
