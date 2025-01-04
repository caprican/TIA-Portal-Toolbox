using System.Windows.Controls;
using System.Windows.Navigation;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.ViewModels;

namespace TiaPortalToolbox.Services;

internal class NavigationService(IPageService pageService) : INavigationService
{
    private readonly IPageService pageService = pageService;
    private Frame? frame;
    private object? lastParameterUsed;

    public event EventHandler<string?>? Navigated;
    public event EventHandler? LoadCompleted;

    public bool CanGoBack => frame?.CanGoBack ?? false;

    public void Initialize(Frame? shellFrame)
    {
        if (frame is null)
        {
            frame = shellFrame;

            if (frame is null) return;
            frame.Navigated += OnNavigated;
            frame.LoadCompleted += OnLoadCompleted;
        }
    }

    public void UnsubscribeNavigation()
    {
        if (frame is not null)
        {
            frame.Navigated -= OnNavigated;
            frame.LoadCompleted -= OnLoadCompleted;
        }
        frame = null;
    }

    public void GoBack()
    {
        if (frame?.CanGoBack == true)
        {
            var vmBeforeNavigation = frame.GetDataContext();
            frame.GoBack();
            if (vmBeforeNavigation is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedFrom();
            }
        }
    }

    public bool NavigateTo(string? pageKey, object? parameter = null, bool clearNavigation = false)
    {
        var pageType = pageService.GetPageType(pageKey);
        if(frame is null) return false;

        if (frame?.Content?.GetType() != pageType || (parameter is not null && parameter.Equals(lastParameterUsed)))
        {
            frame!.Tag = clearNavigation;
            var page = pageService.GetPage(pageKey);
            var navigated = frame.Navigate(page, parameter);
            if (navigated)
            {
                lastParameterUsed = parameter;
                var dataContext = frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }

    public void CleanNavigation() => frame?.CleanNavigation();

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            bool clearNavigation = (bool)frame.Tag;
            if (clearNavigation)
            {
                frame.CleanNavigation();
            }

            var dataContext = frame.GetDataContext();
            if (dataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.ExtraData);
            }

            Navigated?.Invoke(sender, dataContext?.GetType().FullName);
        }
    }

    private void OnLoadCompleted(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            var dataContext = frame.GetDataContext();
            if (dataContext is INavigationAware navigationAware)
            {
                navigationAware.LoadCompleted();
            }

            LoadCompleted?.Invoke(sender, EventArgs.Empty);
        }
    }

    public object? GetPageViewModel() => frame?.GetDataContext();
}