using DocumentFormat.OpenXml.Packaging;

using Microsoft.Extensions.Options;

using TiaPortalOpenness.Models.ProjectTree.Plc;
using TiaPortalOpenness.Models.ProjectTree.Plc.Blocks;

using TiaPortalToolbox.Doc.Contracts.Factories;

namespace TiaPortalToolbox.Doc.Factories;

public class PageFactory(IOptions<Models.DocumentSettings> settings) : IPageFactory
{
    private readonly IOptions<Models.DocumentSettings> settings = settings;

    public Contracts.Builders.IPageBuilder? CreatePage(WordprocessingDocument document, TiaPortalOpenness.Models.ProjectTree.Object projectObject, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? derivedItems = null) =>
        projectObject switch
        {
            Fb fb => new Builders.PlcBlockPageBuilder(settings, document, fb, derivedItems!),
            Fc fc => new Builders.PlcBlockPageBuilder(settings, document, fc, derivedItems!),
            Ob ob => new Builders.PlcBlockPageBuilder(settings, document, ob, derivedItems!),
            TiaPortalOpenness.Models.ProjectTree.Plc.Type type => new Builders.PlcDatatypePageBuilder(settings, document, type, derivedItems!),
            //Core.Models.ProjectTree.Plc.Tag tag => new Builders.PlcTagPageBuilder(settings, document, tag),
            _ => null
        };

    public Contracts.Builders.IPageBuilder? CreatePage(WordprocessingDocument document, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Object> projectObject, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? derivedItems = null) =>
        projectObject switch
        {
            Fb fb => new Builders.PlcBlockPageBuilder(settings, document, fb, derivedItems!),
            Fc fc => new Builders.PlcBlockPageBuilder(settings, document, fc, derivedItems!),
            Ob ob => new Builders.PlcBlockPageBuilder(settings, document, ob, derivedItems!),
            TiaPortalOpenness.Models.ProjectTree.Plc.Type type => new Builders.PlcDatatypePageBuilder(settings, document, type, derivedItems!),
            IEnumerable<Tag> tag => new Builders.PlcTagPageBuilder(settings, document, tag),
            _ => null
        };
}
