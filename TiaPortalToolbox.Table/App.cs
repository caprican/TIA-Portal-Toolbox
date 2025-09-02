using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TiaPortalToolbox.Table;

public class App
{
    public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
    {
        // Builders
        services.AddTransient<Contracts.Builders.ISpreadsheetBuilder, Builders.SpreadsheetBuilder>();
    }
}
