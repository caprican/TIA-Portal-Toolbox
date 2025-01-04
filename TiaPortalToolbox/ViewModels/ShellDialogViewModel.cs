using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TiaPortalToolbox.ViewModels;

public partial class ShellDialogViewModel : ObservableObject
{
    private ICommand? closeCommand;

    public ICommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(OnClose));

    public Action<bool?>? SetResult { get; set; }

    private void OnClose()
    {
        bool result = true;
        SetResult(result);
    }
}
