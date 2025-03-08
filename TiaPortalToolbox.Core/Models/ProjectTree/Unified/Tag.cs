using Siemens.Engineering.HmiUnified.HmiTags;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

public class Tag(HmiTagTable TagTable, string? Path) : Unified.Object(TagTable.Name, Path)
{
    internal HmiTagTable TagTable = TagTable;

    public override string? DisplayName => Name;
}