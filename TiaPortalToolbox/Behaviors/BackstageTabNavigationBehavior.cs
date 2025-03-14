﻿using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using Fluent;
using Microsoft.Xaml.Behaviors;

using TiaPortalToolbox.Contracts.Services;

using TiaPortalToolbox.Contracts.ViewModels;

namespace TiaPortalToolbox.Behaviors;

public class BackstageTabNavigationBehavior : Behavior<BackstageTabControl>
{
    private IPageService? pageService;

    public void Initialize(IPageService pageService)
    {
        this.pageService = pageService;
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is BackstageTabItem tabItem)
        {
            var frame = new Frame()
            {
                Focusable = false,
                NavigationUIVisibility = NavigationUIVisibility.Hidden
            };
            frame.Navigated += OnNavigated;
            tabItem.Content = frame;
            var page = pageService?.GetPage(tabItem.Tag as string);
            frame.Navigate(page);
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (e.Content is FrameworkElement element)
        {
            if (element.DataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.ExtraData);
            }
        }
    }
}
