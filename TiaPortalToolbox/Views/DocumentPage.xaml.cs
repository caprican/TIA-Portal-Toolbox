using System.Windows.Controls;

using TiaPortalToolbox.ViewModels;

namespace TiaPortalToolbox.Views;

public partial class DocumentPage : Page
{
    public DocumentPage(DocumentViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        ribbonTabItem.DataContext = viewModel;
    }
}
