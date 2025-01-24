using System.Windows;
using System.Windows.Controls;

using Fluent;
using MahApps.Metro.Controls;

using TiaPortalToolbox.Behaviors;
using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.ViewModels;

namespace TiaPortalToolbox.Views;

public partial class ShellWindow : MetroWindow, IShellWindow, IRibbonWindow
{
    public RibbonTitleBar TitleBar
    {
        get => (RibbonTitleBar)GetValue(TitleBarProperty);
        private set => SetValue(TitleBarPropertyKey, value);
    }

    private static readonly DependencyPropertyKey TitleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(ShellWindow), new PropertyMetadata());

    public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

    public event EventHandler<Core.Models.ProjectTree.Object>? SelectedItemChanged;

    public ShellWindow(IPageService pageService, ShellViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        DataContext = viewModel;
        navigationBehavior.Initialize(pageService);
        tabsBehavior.Initialize(navigationService);
    }

    public Frame GetNavigationFrame() => shellFrame;

    public RibbonTabsBehavior GetRibbonTabsBehavior() => tabsBehavior;

    public Frame GetRightPaneFrame() => null; //rightPaneFrame;

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not MetroWindow window) return;

        TitleBar = window.FindChild<RibbonTitleBar>("RibbonTitleBar");
        TitleBar?.InvalidateArrange();
        TitleBar?.UpdateLayout();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        tabsBehavior.Unsubscribe();
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if(e.NewValue is Core.Models.ProjectTree.Object item)
        SelectedItemChanged?.Invoke(this, item);
    }
}
