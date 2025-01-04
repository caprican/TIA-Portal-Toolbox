namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public class Unknow : Devices.Object
{
    public Unknow(string Name, string? Path) : base(Name, Path)
    {

    }

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
