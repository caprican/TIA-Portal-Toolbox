using System.Collections.ObjectModel;
using System.Globalization;

using Siemens.Engineering;
using Siemens.Engineering.Multiuser;

using TiaPortalOpenness.Models.ProjectTree;

namespace TiaPortalOpenness.Contracts.Services;

public interface IOpennessService
{
    public List<string>? EngineeringVersions { get; }
    public Version? EngineeringVersion { get; }

    public List<CultureInfo>? ProjectLanguages { get; }
    public CultureInfo? EditingLanguage { get; }
    public CultureInfo? ReferenceLanguage { get; }

    public ObservableCollection<Models.ProjectTree.Object>? ProjectTreeItems { get; }

    public string? ProjectPath { get; }
    public string? ProjectName { get; }
    public string? UserFolder { get; }
    public string? ExportFolder { get; }

    event EventHandler<string>? ProjectOpenning;
    event EventHandler? NewProjectOpenned;
    event EventHandler<string>? ExportBlocksEnded;

    public void Initialize(string engineeringVersion, string opennessVersion);
    public List<string>? GetOpennessVersions(string? engineeringVersion = null);

    public List<TiaPortalToolbox.Core.Models.TiaProcess> GetProcesses();

    public Task OpenOrAttachProjectAsync(TiaPortalToolbox.Core.Models.TiaProcess? process = null, string? path = null);
    public Task<List<Connexion>> GetCommunDataBlock(string markBlock);
    public Task<string[]?> ExportAsync(Models.ProjectTree.Object? projectItem, string? path = null);
    internal Task<string[]?> ExportAsync(IEngineeringObject projectItem, string? path = null);

    public Task<Models.ProjectTree.Object?> GetItem(string blockName);
}
