using System.Globalization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TiaPortalToolbox.Core.Contracts.Services;

using TiaPortalToolbox.Core.Services;
using TiaPortalToolbox.Doc.Builders;

AppDomain.CurrentDomain.AssemblyResolve += TiaPortalToolbox.Core.Helpers.Resolver.OnResolve;
AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += TiaPortalToolbox.Core.Helpers.Resolver.OnResolve;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((services) =>
    {
        services.AddHostedService<Worker>();

        services.AddOptions<TiaPortalToolbox.Doc.Models.DocumentSettings>();

        services.AddSingleton<IOpennessService, OpennessService>();
        services.AddTransient<IPlcService, PlcService>();

        services.AddTransient<TiaPortalToolbox.Doc.Contracts.Builders.IDocumentBuilder, DocumentBuilder>();

        services.AddTransient<TiaPortalToolbox.Doc.Contracts.Factories.IPageFactory, TiaPortalToolbox.Doc.Factories.PageFactory>();
    })
    .Build();

Console.WriteLine("Starting");
await host.RunAsync();
Console.WriteLine("Stopped");

public class Worker(IHostApplicationLifetime hostApplicationLifetime, IOpennessService opennessService, IPlcService plcService,
                    TiaPortalToolbox.Doc.Contracts.Builders.IDocumentBuilder documentBuilder) : BackgroundService
{
    private readonly IHostApplicationLifetime hostApplicationLifetime = hostApplicationLifetime;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly TiaPortalToolbox.Doc.Contracts.Builders.IDocumentBuilder documentBuilder = documentBuilder;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("RunAsync starting");

        opennessService.Initialize("18.0", "18.0");
        var projectName = "SKF libraries";

        List<TiaPortalToolbox.Core.Models.ProjectTree.Object> projectItems = [];
        List<TiaPortalToolbox.Core.Models.ProjectTree.Object> derivedItems = [];

        var fileName = @$"C:\Users\capri\OneDrive\Documents\Automation\TIA069_1\UserFiles\Export\LSKF_Motor.xml";
        projectItems.Add(plcService.GetMetaDataBlock(fileName));

        fileName = @$"C:\Users\capri\OneDrive\Documents\Automation\TIA069_1\UserFiles\Export\LSKF_typeMotor.xml";
        projectItems.Add(plcService.GetMetaDataBlock(fileName));

        var culture = CultureInfo.GetCultureInfo("fr-FR");
        if (File.Exists(@$"C:/Users/capri/Downloads/{projectName}_{culture.Name}.docx"))
            File.Delete(@$"C:/Users/capri/Downloads/{projectName}_{culture.Name}.docx");

        //File.Copy("templatePath", @$"C:/Users/capri/Downloads/{blockName}_{culture.Name}.docx");

        await documentBuilder.CreateDocument(projectItems, derivedItems, culture, @$"C:/Users/capri/Downloads/{projectName}_{culture.Name}.docx");
        documentBuilder.Save();

        Console.WriteLine();
        Console.WriteLine("RunAsync done");
        hostApplicationLifetime.StopApplication();
    }
}