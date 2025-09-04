using System.Globalization;

namespace TiaPortalToolbox.Doc.Contracts.Builders;

public interface IDocumentBuilder
{
    public Task CreateDocument(List<TiaPortalOpenness.Models.ProjectTree.Object> projetItems, List<TiaPortalOpenness.Models.ProjectTree.Object> derivedItems);
    public void Save();


}
