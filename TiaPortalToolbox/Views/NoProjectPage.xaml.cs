using System.Windows.Controls;

using TiaPortalToolbox.ViewModels;

namespace TiaPortalToolbox.Views;

public partial class NoProjectPage : Page
{
    public NoProjectPage(NoProjectViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
