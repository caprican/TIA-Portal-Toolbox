using System.Globalization;

namespace TiaPortalToolbox.Core.Models;

public class InterfaceMember
{
    public string? Name { get; set; }

    public DirectionMember Direction { get; set; }

    public string? Type { get; set; }

    public string? DerivedType { get; set; } = null;

    public Dictionary<CultureInfo, string>? Descriptions { get; set; }

    public string DefaultValue { get; set; } = string.Empty;

    public bool IsConstant { get; set; } = false;

    public bool Islocked { get; set; } = false;

    public bool HidenInterface { get; set; } = false;

    public List<InterfaceMember>? Members { get; set; } = null;
}
