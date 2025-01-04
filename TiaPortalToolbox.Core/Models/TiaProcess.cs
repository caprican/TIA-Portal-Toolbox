namespace TiaPortalToolbox.Core.Models;

public record TiaProcess
{
    /// <summary>
    /// Gets the process ID of TIA Portal.
    /// </summary>
    public int Id { get; private set; }

    public FileInfo? ProjectOpen { get; private set; }

    public string ProjectName => ProjectOpen?.Name ?? "No project";

    public TiaProcess(int id, FileInfo? project)
    {
        Id = id;
        ProjectOpen = project;
    }
}
