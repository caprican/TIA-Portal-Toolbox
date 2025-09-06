using System.Diagnostics;
using System.Globalization;

namespace TiaPortalOpenness.Models;

[DebuggerDisplay("{FolderName}/{DataBlockName}.{FullName}")]
internal record HmiDataBlockTag
{
    internal string? FullName { get; set; }
    internal string? DataBlockName { get; set; }
    internal string? ClassAlarm { get; set; }
    internal Dictionary<CultureInfo, string>? Descriptions { get; set; }
    internal string? FolderName { get; set; }

    internal string? Tagname { get; set; }
    internal string? HmiTagname { get; set; }
}
