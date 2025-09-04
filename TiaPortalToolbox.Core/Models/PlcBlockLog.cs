namespace TiaPortalToolbox.Core.Models;

public record PlcBlockLog
{
    public string? Author { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public DateTime? Edited { get; set; }
}
