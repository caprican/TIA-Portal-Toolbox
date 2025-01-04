using Siemens.Engineering.HW;
using System.Globalization;
using TiaPortalToolbox.Openness.Models;

namespace TiaPortalToolbox.Core.Contrats.Services;

public interface IOpennessService
{
    List<string>? EngineeringVersions { get; }
    Version? EngineeringVersion { get; }

    //public List<CultureInfo>? ProjectLanguages { get; }
    //public CultureInfo? ReferenceLanguage { get; }

    //string ProjectPath { get; }

    //string? ProjectName { get; }

    void Initialize(string? engineeringVersion, string? opennessVersion);

    void Start();

    List<TiaProcess> GetProcesses();

    void Connect(TiaProcess process);

    //void Close();

    List<string>? GetOpennessVersions(string engineeringVersion = "");

    //bool OpenProject(string path);

    //event EventHandler? ProjectOpened;
    //event EventHandler<string>? ExportBlocksEnded;

    //List<DeviceComposition>? GetDevices();

    //Task ExportAsync(string blockPath);

    //public string ExportBlock(Models.Devices.Compositions.Block block, string plc = "", string path = "");

    //public void EndExculsiveAccess();
}
