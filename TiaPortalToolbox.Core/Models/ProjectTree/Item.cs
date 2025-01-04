using System.Diagnostics;

namespace TiaPortalToolbox.Core.Models.ProjectTree;

[DebuggerDisplay("{DisplayName}")]
public class Item(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
    public override string? DisplayName => Name;

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
