using Siemens.Engineering.Hmi.Tag;

namespace TiaPortalOpenness.Models.ProjectTree.Hmi;

public class TagTable(Siemens.Engineering.Hmi.Tag.TagTable HmiTag, string Path) : Object(HmiTag.Name, Path)
{
    internal Siemens.Engineering.Hmi.Tag.TagTable HmiTag = HmiTag;

    public override string? DisplayName => throw new NotImplementedException();
}
