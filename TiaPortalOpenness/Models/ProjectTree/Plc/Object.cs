using System.Diagnostics;
using System.Globalization;

using TiaPortalToolbox.Core.Models;

namespace TiaPortalOpenness.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public abstract class Object(string Name, string? Path) : ProjectTree.Object(Name, Path)
{
    public List<Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>>? Comments { get; set; }

    public Dictionary<CultureInfo, string>? Title { get; set; }
    public Dictionary<CultureInfo, string>? Author { get; set; }
    public Dictionary<CultureInfo, string>? Comment { get; set; }
    public Dictionary<CultureInfo, string>? Function { get; set; }
    public Dictionary<CultureInfo, string>? Library { get; set; }
    public Dictionary<CultureInfo, string>? Family { get; set; }
    public Dictionary<CultureInfo, string>? Preamble { get; set; }
    public Dictionary<CultureInfo, string>? Descriptions { get; set; }
    public Dictionary<CultureInfo, string>? Appendix { get; set; }

    public bool IsSafetyBlock { get; set; } = false;

    public string? Version { get; set; } = null;

    public Dictionary<CultureInfo, List<InterfaceMember>>? Members { get; set; }

    public Dictionary<CultureInfo, List<PlcBlockLog>>? Logs { get; set; }
}
