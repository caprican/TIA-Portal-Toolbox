namespace TiaPortalToolbox.Core.Contracts.Factories;

public interface IPageFactory
{
    public Builders.IPageBuilder? CreatePage(Models.ProjectTree.Object projectObject, IEnumerable<Models.ProjectTree.Plc.Object>? derivedItems = null);
    public Builders.IPageBuilder? CreatePage(IEnumerable<Models.ProjectTree.Object> projectObject, IEnumerable<Models.ProjectTree.Plc.Object>? derivedItems = null);
}

