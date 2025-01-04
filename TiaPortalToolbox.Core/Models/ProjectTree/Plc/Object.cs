using System.Diagnostics;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
}
