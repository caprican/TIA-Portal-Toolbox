namespace TiaPortalToolbox.Openness.Models;

public class TiaProcess(int id, FileInfo? project)
{
    /// <summary>
    /// Gets the process ID of TIA Portal.
    /// </summary>
    public int Id { get; private set; } = id;

    public FileInfo? ProjectOpen { get; private set; } = project;

    public string ProjectName => ProjectOpen?.Name ?? "No project";
}
