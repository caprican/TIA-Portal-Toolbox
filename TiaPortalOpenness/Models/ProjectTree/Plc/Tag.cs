using System.Diagnostics;

using Siemens.Engineering.SW.Tags;

namespace TiaPortalOpenness.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public class Tag : Object
{
    internal PlcTagTable? TagTable;

    public override string? DisplayName => Name;

    public Tag(PlcTagTable? tagTable, string? Path) : base(tagTable?.Name ?? string.Empty, Path)
    {
        TagTable = tagTable;
    }

    public Tag(SimaticML.SW.Tags.PlcTagTable plcTagTable, string? Path) : base(plcTagTable.Attributes.Name, Path)
    {
        TagTable = null;
    }
}
