using Microsoft.Extensions.Hosting;

using Siemens.Collaboration.Net;

using TiaPortalToolbox.Interface.Contrats.Services;

namespace TiaPortalToolbox.Interface.Services;

internal class ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<Core.Contracts.Activation.IActivationHandler> activationHandlers
                                     , IOpennessService opennessService) : IHostedService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly IEnumerable<Core.Contracts.Activation.IActivationHandler> activationHandlers = activationHandlers;
    private readonly IOpennessService opennessService = opennessService;

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

    private async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            opennessService.Initialize();

            await Task.CompletedTask;
        }
    }

    private async Task StartupAsync()
    {
        if (!isInitialized)
        {
            //opennessService.Initialize();
            await Task.CompletedTask;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task HandleActivationAsync()
    {
        var activationHandler = activationHandlers.FirstOrDefault(h => h.CanHandle());

        if (activationHandler is not null)
        {
            await activationHandler.HandleAsync();
        }
        await Task.CompletedTask;

    }
}
