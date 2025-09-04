using System.Diagnostics;

namespace TiaPortalOpenness.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public class Type : Object
{
    internal Siemens.Engineering.SW.Types.PlcType? PlcType;

    public override string? DisplayName => Name;

    public Type(Siemens.Engineering.SW.Types.PlcType? Type, string? Path) : base(Type?.Name ?? "", Path)
    {
        PlcType = Type;
    }

    public Type(SimaticML.SW.Types.PlcStruct plcStruct, string? Path) : base(plcStruct.Attributes.Name, Path)
    {
        PlcType = null;
    }
}
