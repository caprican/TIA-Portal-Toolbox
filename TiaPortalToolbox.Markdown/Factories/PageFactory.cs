using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Factories;

namespace TiaPortalToolbox.Doc.Factories;

public class PageFactory(IOptions<Models.DocumentSettings> settings) : IPageFactory
{
    private readonly IOptions<Models.DocumentSettings> settings = settings;

    public Contracts.Builders.IPageBuilder? CreatePage(Core.Models.ProjectTree.Object projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems) =>
        projectObject switch
        {
            Core.Models.ProjectTree.Plc.Blocks.Fb fb => new Builders.PlcBlockPageBuilder(settings, fb, derivedItems),
            Core.Models.ProjectTree.Plc.Blocks.Fc fc => new Builders.PlcBlockPageBuilder(settings, fc, derivedItems),
            Core.Models.ProjectTree.Plc.Blocks.Ob ob => new Builders.PlcBlockPageBuilder(settings, ob, derivedItems),
            Core.Models.ProjectTree.Plc.Type type => new Builders.PlcDatatypePageBuilder(settings, type, derivedItems),
            _ => null
        };
}
