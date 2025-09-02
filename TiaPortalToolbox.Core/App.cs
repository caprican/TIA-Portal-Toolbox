using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TiaPortalToolbox.Core;

public class App
{
    public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<Contracts.Services.IOpennessService, Services.OpennessService>();

        services.AddTransient<Contracts.Services.IPlcService, Services.PlcService>();
        services.AddTransient<Contracts.Services.IHmiService, Services.HmiService>();
        services.AddTransient<Contracts.Services.IUnifiedService, Services.UnifiedService>();
    }
}
