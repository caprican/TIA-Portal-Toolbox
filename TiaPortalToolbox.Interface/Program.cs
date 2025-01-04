using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TiaPortalToolbox.Interface.Contrats.Services;
using TiaPortalToolbox.Interface.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
    .Build();

host.Run();

Console.ReadLine();

void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddHostedService<ApplicationHostService>();
    services.AddSingleton<IOpennessService, OpennessService>();
}