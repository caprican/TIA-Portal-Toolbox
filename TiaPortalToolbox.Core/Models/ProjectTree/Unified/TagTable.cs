using System.Diagnostics;

using Siemens.Engineering.HmiUnified.HmiTags;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

[DebuggerDisplay("{DisplayName}")]
public class TagTable(HmiTagTable HmiTag, string Path) : Unified.Object(HmiTag.Name, Path)
{
    public override string? DisplayName => Name;
}
