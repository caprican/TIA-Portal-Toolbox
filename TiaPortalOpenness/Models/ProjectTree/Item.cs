using System.Diagnostics;

namespace TiaPortalOpenness.Models.ProjectTree;

[DebuggerDisplay("{DisplayName}")]
public class Item(string Name, string? Path) : Object(Name, Path)
{
    public override string? DisplayName => Name;
}
