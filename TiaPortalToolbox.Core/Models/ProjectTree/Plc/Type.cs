using System.Diagnostics;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public class Type(Siemens.Engineering.SW.Types.PlcType? Type, string? Path) : Plc.Object(Type?.Name ?? "", Path)
{
    internal Siemens.Engineering.SW.Types.PlcType? PlcType = Type;

    public override string? DisplayName => Name;
}
