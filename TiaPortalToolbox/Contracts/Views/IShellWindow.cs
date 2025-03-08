using MahApps.Metro.Controls;
using System.Windows.Controls;
using TiaPortalToolbox.Behaviors;

namespace TiaPortalToolbox.Contracts.Views;

public interface IShellWindow
{
    public event EventHandler<Core.Models.ProjectTree.Object?>? SelectedItemChanged;

    public Core.Models.ProjectTree.Object? SelectedItem { get; }

    Frame? GetNavigationFrame();

    void ShowWindow();

    void CloseWindow();

    Frame GetRightPaneFrame();

    RibbonTabsBehavior GetRibbonTabsBehavior();
}
