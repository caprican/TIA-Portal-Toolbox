using DocumentFormat.OpenXml.Packaging;

namespace TiaPortalToolbox.Doc.Contracts.Factories;

public interface IPageFactory
{
    public Builders.IPageBuilder? CreatePage(WordprocessingDocument document, TiaPortalOpenness.Models.ProjectTree.Object projectObject, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? derivedItems = null);
    public Builders.IPageBuilder? CreatePage(WordprocessingDocument document, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Object> projectObject, IEnumerable<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? derivedItems = null);
}
