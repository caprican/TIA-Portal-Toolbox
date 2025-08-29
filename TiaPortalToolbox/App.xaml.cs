using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Core.Services;
using TiaPortalToolbox.Models;
using TiaPortalToolbox.Services;
using TiaPortalToolbox.ViewModels;
using TiaPortalToolbox.Views;

namespace TiaPortalToolbox;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : Application
{
    private IHost? host;

    public T? GetService<T>() where T : class => host?.Services.GetService(typeof(T)) as T;

    public App()
    {
        AppDomain.CurrentDomain.AssemblyResolve += Core.Helpers.Resolver.OnResolve;
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += Core.Helpers.Resolver.OnResolve;
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
        host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation!);
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureServices(OpennessConfigureService)
                .Build();

        await host.StartAsync();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // App Host
        services.AddHostedService<ApplicationHostService>();
        services.AddSingleton<IDialogCoordinator, DialogCoordinator>();

        // Activation Handlers

        // Core Services
        services.AddSingleton<IFileService, FileService>();

        // Services
        services.AddSingleton<IWindowManagerService, WindowManagerService>();
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<ICultureSelectorService, CultureSelectorService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<INavigationService, NavigationService>();
        //services.AddSingleton<ISampleDataService, SampleDataService>();

        // Factories
        services.AddTransient<Doc.Contracts.Factories.IPageFactory, Doc.Factories.PageFactory>();
        //services.AddTransient<Core.Contracts.Factories.IPageFactory, Core.Factories.PageFactory>();

        // Builders
        services.AddTransient<Doc.Contracts.Builders.IDocumentBuilder, Doc.Builders.DocumentBuilder>();
        //services.AddTransient<Core.Contracts.Builders.IDocumentBuilder, Core.Builders.DocumentBuilder>();

        // Views and ViewModels
        services.AddSingleton<IShellWindow, ShellWindow>();
        services.AddSingleton<ShellViewModel>();

        services.AddTransient<IShellDialogWindow, ShellDialogWindow>();
        services.AddTransient<ShellDialogViewModel>();

        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();

        services.AddTransient<NoProjectViewModel>();
        services.AddTransient<NoProjectPage>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<MainPage>();

        services.AddTransient<BuildHmiAlarmsViewModel>();
        services.AddTransient<BuildHmiAlarmsPage>();

        services.AddTransient<DocumentViewModel>();
        services.AddTransient<DocumentPage>();

        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

    }

    private void OpennessConfigureService(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IOpennessService, OpennessService>();

        services.AddTransient<IPlcService, PlcService>();
        services.AddTransient<IHmiService, HmiService>();
        services.AddTransient<IUnifiedService, UnifiedService>();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await host?.StopAsync();
        host.Dispose();
        host = null;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
