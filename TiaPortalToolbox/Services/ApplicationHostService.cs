using Microsoft.Extensions.Hosting;

using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;

namespace TiaPortalToolbox.Services;

internal class ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<Core.Contracts.Activation.IActivationHandler> activationHandlers,
                                      INavigationService navigationService,
                                    //IRightPaneService rightPaneService, IProjectNavigatorService projectNavigatorService,
                                    IThemeSelectorService themeSelectorService,
                                    ISettingsService settingsService, IPersistAndRestoreService persistAndRestoreService,

                                    IOpennessService opennessService/*, IPlcProjectService plcProjectService*/) : IHostedService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly INavigationService navigationService = navigationService;
    private readonly ISettingsService settingsService = settingsService;
    private readonly IPersistAndRestoreService persistAndRestoreService = persistAndRestoreService;
    private readonly IThemeSelectorService themeSelectorService = themeSelectorService;
    //private readonly IRightPaneService rightPaneService = rightPaneService;
    //private readonly IProjectNavigatorService projectNavigatorService = projectNavigatorService;

    private readonly IOpennessService opennessService = opennessService;

    private readonly IEnumerable<Core.Contracts.Activation.IActivationHandler> activationHandlers = activationHandlers;
    private IShellWindow? shellWindow;
    private bool isInitialized;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize services that you need before app activation
        await InitializeAsync();

        await HandleActivationAsync();

        // Tasks after activation
        await StartupAsync();
        isInitialized = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        persistAndRestoreService.PersistData();
        await Task.CompletedTask;
    }

    private async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            persistAndRestoreService.RestoreData();
            themeSelectorService.InitializeTheme();
            settingsService.InitializeSettings();

            await Task.CompletedTask;
        }
    }

    private async Task StartupAsync()
    {
        if (!isInitialized)
        {
            shellWindow?.ShowWindow();
            await Task.CompletedTask;
        }
    }

    private async Task HandleActivationAsync()
    {
        var activationHandler = activationHandlers.FirstOrDefault(h => h.CanHandle());

        if (activationHandler is not null)
        {
            await activationHandler.HandleAsync();
        }

        await Task.CompletedTask;

        if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
        {
            // Default activation that navigates to the apps default page
            shellWindow = serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
            navigationService.Initialize(shellWindow?.GetNavigationFrame());
            //rightPaneService.Initialize(shellWindow.GetRightPaneFrame(), shellWindow.GetSplitView());

            shellWindow?.ShowWindow();

            //navigationService.NavigateTo(typeof(MainViewModel).FullName);

            await Task.CompletedTask;
        }
    }
}
