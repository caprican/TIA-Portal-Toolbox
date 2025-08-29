using DocumentFormat.OpenXml.Packaging;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Factories;

namespace TiaPortalToolbox.Doc.Factories;

public class PageFactory(IOptions<Models.DocumentSettings> settings) : IPageFactory
{
    private readonly IOptions<Models.DocumentSettings> settings = settings;

    public Contracts.Builders.IPageBuilder? CreatePage(WordprocessingDocument document, Core.Models.ProjectTree.Object projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object>? derivedItems = null) =>
        projectObject switch
        {
            Core.Models.ProjectTree.Plc.Blocks.Fb fb => new Builders.PlcBlockPageBuilder(settings, document, fb, derivedItems!),
            Core.Models.ProjectTree.Plc.Blocks.Fc fc => new Builders.PlcBlockPageBuilder(settings, document, fc, derivedItems!),
            Core.Models.ProjectTree.Plc.Blocks.Ob ob => new Builders.PlcBlockPageBuilder(settings, document, ob, derivedItems!),
            Core.Models.ProjectTree.Plc.Type type => new Builders.PlcDatatypePageBuilder(settings, document, type, derivedItems!),
            //Core.Models.ProjectTree.Plc.Tag tag => new Builders.PlcTagPageBuilder(settings, document, tag),
            _ => null
        };

    public Contracts.Builders.IPageBuilder? CreatePage(WordprocessingDocument document, IEnumerable<Core.Models.ProjectTree.Object> projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object>? derivedItems = null) =>
        projectObject switch
        {
            Core.Models.ProjectTree.Plc.Blocks.Fb fb => new Builders.PlcBlockPageBuilder(settings, document, fb, derivedItems!),
            Core.Models.ProjectTree.Plc.Blocks.Fc fc => new Builders.PlcBlockPageBuilder(settings, document, fc, derivedItems!),
            Core.Models.ProjectTree.Plc.Blocks.Ob ob => new Builders.PlcBlockPageBuilder(settings, document, ob, derivedItems!),
            Core.Models.ProjectTree.Plc.Type type => new Builders.PlcDatatypePageBuilder(settings, document, type, derivedItems!),
            IEnumerable<Core.Models.ProjectTree.Plc.Tag> tag => new Builders.PlcTagPageBuilder(settings, document, tag),
            _ => null
        };
}
