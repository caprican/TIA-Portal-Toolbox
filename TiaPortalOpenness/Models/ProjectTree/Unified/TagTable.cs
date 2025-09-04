using System.Diagnostics;

using Siemens.Engineering.HmiUnified.HmiTags;

namespace TiaPortalOpenness.Models.ProjectTree.Unified;

[DebuggerDisplay("{DisplayName}")]
public class TagTable(HmiTagTable HmiTag, string Path) : Object(HmiTag.Name, Path)
{
    public override string? DisplayName => Name;
}
