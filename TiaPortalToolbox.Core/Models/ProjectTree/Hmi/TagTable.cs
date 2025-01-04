using Siemens.Engineering.Hmi.Tag;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Hmi;

public class TagTable(Siemens.Engineering.Hmi.Tag.TagTable HmiTag, string Path) : Hmi.Object(HmiTag.Name, Path)
{
    internal Siemens.Engineering.Hmi.Tag.TagTable HmiTag = HmiTag;

    public override string? DisplayName => throw new NotImplementedException();

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
