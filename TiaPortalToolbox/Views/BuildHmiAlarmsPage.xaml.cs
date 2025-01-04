using System.Windows.Controls;

using TiaPortalToolbox.ViewModels;

namespace TiaPortalToolbox.Views;

public partial class BuildHmiAlarmsPage : Page
{
    public BuildHmiAlarmsPage(BuildHmiAlarmsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        groupBox.DataContext = viewModel;
    }
}
