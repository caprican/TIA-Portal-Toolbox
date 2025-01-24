using System.Diagnostics;

using Siemens.Engineering.SW.Types;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public class Type(PlcType Type, string? Path) : Plc.Object(Type.Name, Path)
{
    internal PlcType PlcType = Type;

    public override string? DisplayName => Name;
}
