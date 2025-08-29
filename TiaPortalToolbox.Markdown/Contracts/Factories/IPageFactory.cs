using DocumentFormat.OpenXml.Packaging;

namespace TiaPortalToolbox.Doc.Contracts.Factories;

public interface IPageFactory
{
    public Builders.IPageBuilder? CreatePage(WordprocessingDocument document, Core.Models.ProjectTree.Object projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object>? derivedItems = null);
    public Builders.IPageBuilder? CreatePage(WordprocessingDocument document, IEnumerable<Core.Models.ProjectTree.Object> projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object>? derivedItems = null);
}
