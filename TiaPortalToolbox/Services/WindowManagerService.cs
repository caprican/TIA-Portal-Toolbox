using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using MahApps.Metro.Controls;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.ViewModels;

using TiaPortalToolbox.Contracts.Views;

namespace TiaPortalToolbox.Services;

public class WindowManagerService(IServiceProvider serviceProvider, IPageService pageService) : IWindowManagerService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IPageService pageService = pageService;

    public Window MainWindow => Application.Current.MainWindow;

    public void OpenInNewWindow(string key, object? parameter = null)
    {
        var window = GetWindow(key);
        if (window is not null)
        {
            window.Activate();
        }
        else
        {
            window = new MetroWindow()
            {
                Title = "ToolsAssist",
                Style = Application.Current.FindResource("CustomMetroWindow") as Style
            };
            var frame = new Frame()
            {
                Focusable = false,
                NavigationUIVisibility = NavigationUIVisibility.Hidden
            };

            window.Content = frame;
            var page = pageService.GetPage(key);
            window.Closed += OnWindowClosed;
            window.Show();
            frame.Navigated += OnNavigated;
            var navigated = frame.Navigate(page, parameter);
        }
    }

    public bool? OpenInDialog(string key, object? parameter = null)
    {
        var shellWindow = serviceProvider.GetService(typeof(IShellDialogWindow)) as Window;
        var frame = ((IShellDialogWindow)shellWindow).GetDialogFrame();

        frame.Navigated += OnNavigated;
        shellWindow.Closed += OnWindowClosed;
        
        var page = pageService.GetPage(key);
        var navigated = frame.Navigate(page, parameter);
        return shellWindow.ShowDialog();
    }

    public Window? GetWindow(string key)
    {
        foreach (Window window in Application.Current.Windows)
        {
            var dataContext = window.GetDataContext();
            if (dataContext?.GetType().FullName == key)
            {
                return window;
            }
        }

        return null;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            var dataContext = frame.GetDataContext();
            if (dataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.ExtraData);
            }
        }
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        if (sender is Window window)
        {
            if (window.Content is Frame frame)
            {
                frame.Navigated -= OnNavigated;
            }

            window.Closed -= OnWindowClosed;
        }
    }
}
