using Siemens.Engineering.SW.Tags;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc;

public class Tag(PlcTagTable TagTable, string? Path) : Plc.Object(TagTable.Name, Path)
{
    internal PlcTagTable TagTable = TagTable;

    public override string? DisplayName => Name;
}
