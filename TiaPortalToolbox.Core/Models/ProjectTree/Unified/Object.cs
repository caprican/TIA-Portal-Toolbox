using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

[DisplayName("{Name}")]
public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
}
