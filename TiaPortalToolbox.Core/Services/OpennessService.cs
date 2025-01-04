using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Multiuser;
using Siemens.Engineering.SW;

using TiaPortalToolbox.Core.Models.ProjectTree;
using TiaPortalToolbox.Core.Models.ProjectTree.Devices;

namespace TiaPortalToolbox.Core.Services;

public class OpennessService : Contracts.Services.IOpennessService
{
    private Version? engineeringVersion;
    private TiaPortal? tiaPortal;
    private ProjectBase? project;
    private List<CultureInfo>? projectLanguages;
    private CultureInfo? editingLanguage;
    private CultureInfo? referenceLanguage;
    private ObservableCollection<Models.ProjectTree.Object>? projectsTreeItems;

    public List<string>? EngineeringVersions { get; }
    public Version? EngineeringVersion => engineeringVersion;
    public List<CultureInfo>? ProjectLanguages => projectLanguages;
    public CultureInfo? EditingLanguage => editingLanguage;
    public CultureInfo? ReferenceLanguage => referenceLanguage;
    public string? ProjectPath => project?.Path.DirectoryName;
    public string? ProjectName => project?.Name;
    public string? UserFolder => !string.IsNullOrEmpty(ProjectPath) ? Path.Combine(ProjectPath,  "UserFiles") : null;
    public string? ExportFolder => !string.IsNullOrEmpty(ProjectPath) ? Path.Combine(UserFolder,  "Export") : null;

    public ObservableCollection<Models.ProjectTree.Object>? ProjectTreeItems => projectsTreeItems;


    public event EventHandler<string>? ProjectOpenning;
    public event EventHandler? NewProjectOpenned;
    public event EventHandler<string>? ExportBlocksEnded;

    public OpennessService()
    {
        EngineeringVersions = Helpers.Resolver.GetEngineeringVersions();
    }

    public void Initialize(string engineeringVersion, string opennessVersion)
    {
        if (Helpers.Resolver.GetAssemblyPath(engineeringVersion, opennessVersion) != null)
        {
            this.engineeringVersion = new Version(engineeringVersion);
        }
        else
        {
            this.engineeringVersion = null;
        }

    }

    public List<string>? GetOpennessVersions(string? engineeringVersion = null)
    {
        if (!string.IsNullOrEmpty(engineeringVersion))
        {
            return Helpers.Resolver.GetAssemblies(engineeringVersion ?? string.Empty);
        }

        return null;
    }

    public void Close()
    {
        if (tiaPortal != null)
        {
            tiaPortal.Confirmation -= TiaPortal_Confirmation;
            tiaPortal.Authentication -= TiaPortal_Authentication;
            tiaPortal.Notification -= TiaPortal_Notification;
            tiaPortal.Disposed -= TiaPortal_Disposed;

            tiaPortal.Dispose();
            tiaPortal = null;
        }
    }

    private void TiaPortal_Disposed(object sender, System.EventArgs e)
    {
        if (tiaPortal is not null)
        {
            tiaPortal.Confirmation -= TiaPortal_Confirmation;
            tiaPortal.Authentication -= TiaPortal_Authentication;
            tiaPortal.Notification -= TiaPortal_Notification;
            tiaPortal.Disposed -= TiaPortal_Disposed;

            tiaPortal.Dispose();
            tiaPortal = null;
        }
    }

    private void TiaPortal_Notification(object sender, NotificationEventArgs e)
    {

    }

    private void TiaPortal_Authentication(object sender, AuthenticationEventArgs e)
    {

    }

    private void TiaPortal_Confirmation(object sender, ConfirmationEventArgs e)
    {

    }

    public List<Models.TiaProcess> GetProcesses()
    {
        var tiaProcesses = new List<Models.TiaProcess>();
        foreach (var process in Siemens.Engineering.TiaPortal.GetProcesses())
        {
            tiaProcesses.Add(new Models.TiaProcess(process.Id, process.ProjectPath));
        }
        return tiaProcesses;
    }

    public Task OpenOrAttachProjectAsync(Models.TiaProcess? process = null, string? path = null)
    {
        var tcs = new TaskCompletionSource<ProjectBase?>();
        ThreadPool.QueueUserWorkItem(async _ =>
        {
            try
            {
                Helpers.Resolver.SetWhiteList(engineeringVersion);

                if (process is null)
                {
                    var tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);

                    if(!string.IsNullOrEmpty(path))
                    {
                        if (engineeringVersion is null) return;

                        var fileInfo = new FileInfo(path);
                        ProjectOpenning?.Invoke(this, fileInfo.Name);

                        var version = engineeringVersion.Major.ToString();
                        if (engineeringVersion.Minor > 0)
                        {
                            version += $"_{engineeringVersion.Minor}";
                        }

                        switch (fileInfo.Extension.Replace(version, ""))
                        {
                            case ".ap":
                                project = tiaPortal?.Projects.Open(fileInfo);
                                break;               // Open project
                            case ".als":
                                //if (tiaPortal?.ProjectServers != null)
                                //{
                                //    foreach (var server in tiaPortal?.ProjectServers)
                                //    {

                                //        foreach (var project in server.GetServerProjects())
                                //        {
                                //            server.GetLocalSessions(project);
                                //        }
                                //    }
                                //}
                                break;              // Open local session project
                            case ".amc":
                                //var p = tiaPortal?.LocalSessions.OpenServerProject(projectPath);
                                break;              // Open multiuser project
                        }

                        //NewProjectOpenned?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    tiaPortal = TiaPortal.GetProcess(process.Id).Attach();
                    project = tiaPortal?.Projects?.FirstOrDefault(f => f.IsPrimary);
                    //project ??= tiaPortal?.LocalSessions?.FirstOrDefault(p => p.Project.IsPrimary)?.Project;

                    //NewProjectOpenned?.Invoke(this, EventArgs.Empty);
                }

                if (tiaPortal is not null)
                {
                    tiaPortal.Confirmation += TiaPortal_Confirmation;
                    tiaPortal.Authentication += TiaPortal_Authentication;
                    tiaPortal.Notification += TiaPortal_Notification;
                    tiaPortal.Disposed += TiaPortal_Disposed;

                    referenceLanguage = project?.LanguageSettings.ReferenceLanguage.Culture;
                    editingLanguage = project?.LanguageSettings.EditingLanguage.Culture;
                    if (project?.LanguageSettings?.ActiveLanguages != null)
                    {
                        foreach (var lang in project.LanguageSettings.ActiveLanguages)
                        {
                            projectLanguages ??= [];
                            projectLanguages.Add(lang.Culture);
                        }
                    }

                    if(project is not null)
                    {
                        projectsTreeItems = [];
                        await GetProjectTreeAsync();
                    }
                    else
                    {
                        projectsTreeItems = null;
                    }

                    NewProjectOpenned?.Invoke(this, EventArgs.Empty);
                }

                tcs.SetResult(project);
            }
            catch (Exception ex) 
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    internal Task GetProjectTreeAsync()
    {
        var tcs = new TaskCompletionSource<ObservableCollection<Models.ProjectTree.Object>?>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if (project is null)
                {
                    projectsTreeItems = null;
                    tcs.SetResult(null);
                }
                else
                {
                    if(GetProjectDevices(project.Devices, string.Empty) is List<Models.ProjectTree.Object> devices)
                    {
                        projectsTreeItems ??= [];
                        projectsTreeItems.AddRange(devices);
                    }

                    if(GetProjectDeviceGroups(project.DeviceGroups, string.Empty) is List<Models.ProjectTree.Object> deviceGroups)
                    {
                        projectsTreeItems ??= [];
                        projectsTreeItems.AddRange(deviceGroups);

                    }

                    if (GetProjectDevices(project.UngroupedDevicesGroup.Devices, string.Empty) is List<Models.ProjectTree.Object> tempUngroupDevices)
                    {
                        projectsTreeItems ??= [];
                        projectsTreeItems.AddRange(tempUngroupDevices);
                    }

                    tcs.SetResult(projectsTreeItems);
                }
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    private List<Models.ProjectTree.Object>? GetProjectDevices(DeviceComposition devices, string parentPath)
    {
        List<Models.ProjectTree.Object>? deviceItems = null;
        if(devices?.Count > 0)
        {
            foreach(var device in devices)
            {
                var deviceAdded = false;
                foreach(var deviceItem in device.DeviceItems)
                {
                    switch(deviceItem.GetService<SoftwareContainer>()?.Software)
                    {
                        case PlcSoftware plcSoftware:
                            deviceAdded = true;
                            deviceItems ??= [];
                            deviceItems.Add(new Models.ProjectTree.Devices.Plc(plcSoftware, Path.Combine(parentPath, device.Name)));
                            break;
                        case HmiTarget hmiTarget:
                            deviceAdded = true;
                            deviceItems ??= [];
                            deviceItems.Add(new Models.ProjectTree.Devices.Hmi(hmiTarget, Path.Combine(parentPath, device.Name)));
                            break;
                        case HmiSoftware hmiSoftware:
                            deviceAdded = true;
                            deviceItems ??= [];
                            deviceItems.Add(new Models.ProjectTree.Devices.Unified(hmiSoftware, Path.Combine(parentPath, device.Name)));
                            break;
                        default:
                            break;
                    }
                }

                if(!deviceAdded)
                {
                    deviceItems ??= [];
                    deviceItems.Add(new Models.ProjectTree.Devices.Unknow(device.Name, Path.Combine(parentPath, device.Name)));
                }
            }
        }

        return deviceItems;
    }

    private List<Models.ProjectTree.Object>? GetProjectDeviceGroups(DeviceUserGroupComposition devicesGroups, string parentPath)
    {
        List<Models.ProjectTree.Object>? groups = null;

        if(devicesGroups?.Count > 0)
        {
            var deviceGroup = devicesGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;

                if (GetProjectDevices(deviceGroup[0].Devices, Path.Combine(groupPath, deviceGroup[0].Name)) is List<Models.ProjectTree.Object> devices)
                {
                    groupItems ??= [];
                    groupItems.AddRange(devices);
                }

                if (deviceGroup[0].Groups?.Count > 0 &&
                    GetProjectDeviceGroups(deviceGroup[0].Groups, Path.Combine(groupPath, deviceGroup[0].Name)) is List<Models.ProjectTree.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if(groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Item(deviceGroup[0].Name, Path.Combine(groupPath, deviceGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                deviceGroup.RemoveAt(0);
            } while (deviceGroup.Count > 0);
        }

        return groups;
    }

    public Task<List<Connexion>> GetCommunDataBlock(string markBlock)
    {
        var tcs = new TaskCompletionSource<List<Connexion>>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                var result = new List<Connexion>();

                var plcDevices = GetItemTree<Plc>(projectsTreeItems);
                var unifiedDevices = GetItemTree<Unified>(projectsTreeItems);
                var hmiDevices = GetItemTree<Hmi>(projectsTreeItems);

                if (hmiDevices?.Count > 0)
                {
                    foreach (var hmiDevice in hmiDevices)
                    {
                        foreach (var connexion in hmiDevice.Device.Connections)
                        {
                            var plcConnected = plcDevices.FirstOrDefault()/*.SingleOrDefault(s => s.Name == connexion)*/;
                            var blocks = GetItemTree<Models.ProjectTree.Plc.Blocks.DataBlocks.DataBlock>(plcConnected.Items, markBlock);
                            result.Add(new Models.ProjectTree.Connexion(connexion, hmiDevice, plcConnected, blocks));
                        }
                    }
                }

                if (unifiedDevices?.Count > 0)
                {
                    foreach (var unifiedDevice in unifiedDevices)
                    {
                        foreach (var connexion in unifiedDevice.Device.Connections)
                        {
                            var plcConnected = plcDevices.SingleOrDefault(s => s.Name == connexion.Partner);
                            var blocks = GetItemTree<Models.ProjectTree.Plc.Blocks.DataBlocks.DataBlock>(plcConnected.Items, markBlock);
                            result.Add(new Models.ProjectTree.Connexion(connexion, unifiedDevice, plcConnected, blocks));
                        }
                    }
                }
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    private Models.ProjectTree.Object? GetItemTreeByName(string ItemName, ObservableCollection<Models.ProjectTree.Object>? projectTrees)
    {
        if (projectTrees?.SingleOrDefault(s => s.Name == ItemName) is Models.ProjectTree.Object projetItem)
        {
            return projetItem;
        }
        else
        {
            if(projectTrees is not null)
            {
                foreach (var item in projectTrees)
                {
                    var itemFound = GetItemTreeByName(ItemName, item.Items);
                    if(itemFound is not null) return itemFound;
                }
            }

        }
        return null;
    }

    private List<T>? GetItemTree<T>(ObservableCollection<Models.ProjectTree.Object>? projectTrees) where T : Models.ProjectTree.Object
    {
        List<T>? result = projectTrees?.OfType<T>().ToList();

        if (projectTrees is not null)
        {
            foreach (var item in projectTrees)
            {
                var itemFound = GetItemTree<T>(item.Items);
                if (itemFound is not null)
                {
                    result ??= [];
                    result.AddRange(itemFound);
                }
            }
        }
        return result;
    }

    private IEnumerable<T>? GetItemTree<T>(IEnumerable<Models.ProjectTree.Object>? projectTrees, string? markBlock = null) where T : Models.ProjectTree.Plc.Object
    {
        IEnumerable<T>? result = projectTrees?.OfType<T>().Where(w => markBlock is null || w.Name.Contains(markBlock));

        if (projectTrees?.Count() > 0)
        {
            Parallel.ForEach(projectTrees, (item) =>
            {
                if (item.Items?.Count > 0)
                {
                    var itemFound = GetItemTree<T>(item.Items, markBlock);
                    if (itemFound?.Count() > 0)
                    {
                        result ??= [];
                        result = result.Union(itemFound);

                    }
                }
            });
        }
        return result;
    }

    public Task<string[]?> ExportAsync(Models.ProjectTree.Object projectItem, string? path = null)
    {
        var tcs = new TaskCompletionSource<string[]?>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                List<string> exportedPaths = [];
                if (string.IsNullOrEmpty(path))
                {
                    path = ExportFolder;
                }

                switch (projectItem)
                {
                    case Models.ProjectTree.Plc.Blocks.Object plcBlock:

                        var exportPath = new FileInfo(Path.Combine(path, $"{projectItem.Name}.xml"));
                        if (!Directory.Exists(exportPath.DirectoryName))
                        {
                            Directory.CreateDirectory(exportPath.DirectoryName);
                        }

                        if (File.Exists(exportPath.FullName))
                        {
                            File.Delete(exportPath.FullName);
                        }

                        plcBlock.PlcBlock.Export(exportPath, ExportOptions.None, DocumentInfoOptions.None);
                        exportedPaths.Add(exportPath.FullName);

                        break;
                    case Models.ProjectTree.Plc.Item plcItem:
                        if (plcItem.Items?.Count > 0)
                        {
                            foreach (var item in plcItem.Items)
                            {
                                exportedPaths.AddRange(ExportAsync(item, path).Result);
                            }
                        }
                        break;
                }
                tcs.SetResult([.. exportedPaths]);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;

    }
}
