using System.Globalization;

namespace TiaPortalToolbox.Core.Contracts.Builders;

public interface IDocumentBuilder
{
    public Task CreateDocument(List<Models.ProjectTree.Object> projetItems, List<Models.ProjectTree.Object> derivedItems, CultureInfo culture, string path);
    public void Save();
}
