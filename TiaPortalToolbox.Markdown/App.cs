using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TiaPortalToolbox.Doc;

public class App
{
    public static void ConfigureService(HostBuilderContext context, IServiceCollection services)
    {
        // Factories
        services.AddTransient<Contracts.Factories.IPageFactory, Factories.PageFactory>();

        // Builders
        services.AddTransient<Contracts.Builders.IDocumentBuilder, Builders.DocumentBuilder>();
    }
}
