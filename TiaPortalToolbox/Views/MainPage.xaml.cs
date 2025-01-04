using System.Windows.Controls;

using TiaPortalToolbox.ViewModels;

namespace TiaPortalToolbox.Views;

public partial class MainPage : Page
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
