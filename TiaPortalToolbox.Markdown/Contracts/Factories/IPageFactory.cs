namespace TiaPortalToolbox.Doc.Contracts.Factories;

public interface IPageFactory
{
    public Contracts.Builders.IPageBuilder? CreatePage(Core.Models.ProjectTree.Object projectObject, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems);
}
