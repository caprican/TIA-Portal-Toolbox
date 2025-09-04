namespace TiaPortalOpenness.Models.ProjectTree.Devices;

public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
    public override string? DisplayName => Name;

}
