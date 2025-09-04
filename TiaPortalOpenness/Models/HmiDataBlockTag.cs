using System.Diagnostics;
using System.Globalization;

namespace TiaPortalOpenness.Models;

[DebuggerDisplay("{FolderName}/{DataBlockName}.{FullName}")]
internal class HmiDataBlockTag
{
    internal string? FullName { get; set; }
    internal string? DataBlockName { get; set; }
    internal string? ClassAlarm { get; set; }
    internal Dictionary<CultureInfo, string>? Descriptions { get; set; }
    internal string? FolderName { get; set; }
}
