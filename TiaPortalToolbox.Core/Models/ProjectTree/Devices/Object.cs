using System.Collections.ObjectModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
    public override string? DisplayName => Name;

}
