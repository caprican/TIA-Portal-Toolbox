using System.Diagnostics;
using System.Globalization;

namespace TiaPortalOpenness.Models;

[DebuggerDisplay("{FolderName}/{DataBlockName}.{FullName}")]
public record HmiDataBlockTag
{
    public uint Id { get; set; } = 1;
    public string? FullName { get; set; }
    public string? DataBlockName { get; set; }
    public string? ClassAlarm { get; set; }
    public Dictionary<CultureInfo, string>? Descriptions { get; set; }
    public string? FolderName { get; set; }

    public string? Tagname { get; set; }
    public string? HmiTagname { get; set; }
    public string? Datatype { get; set; }
}
