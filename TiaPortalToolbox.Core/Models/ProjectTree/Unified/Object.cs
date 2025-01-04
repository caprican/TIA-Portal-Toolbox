using System.Collections.ObjectModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
