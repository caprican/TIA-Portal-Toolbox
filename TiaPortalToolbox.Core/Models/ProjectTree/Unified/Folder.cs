using System.ComponentModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

[DisplayName("../{DisplayName}")]
public class Folder(string Name, string? Path) : Unified.Object(Name, Path)
{
    public override string? DisplayName => Name;
}
