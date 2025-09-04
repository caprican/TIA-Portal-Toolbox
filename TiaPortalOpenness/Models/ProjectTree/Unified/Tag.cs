using Siemens.Engineering.HmiUnified.HmiTags;

namespace TiaPortalOpenness.Models.ProjectTree.Unified;

public class Tag(HmiTagTable TagTable, string? Path) : Object(TagTable.Name, Path)
{
    internal HmiTagTable TagTable = TagTable;

    public override string? DisplayName => Name;
}