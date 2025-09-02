using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;

using Siemens.Collaboration.Net.CoreExtensions.Conversion;
using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HmiUnified.HmiAlarm;
using Siemens.Engineering.HmiUnified.HmiTags;
using Siemens.Engineering.HmiUnified.UI.Dynamization.Tag;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Multiuser;
using Siemens.Engineering.Settings;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;

using TiaPortalToolbox.Core.Helpers;
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
                    tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);

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

    public Task<string[]?> ExportAsync(Models.ProjectTree.Object? projectItem, string? path = null)
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

                var exportPath = new FileInfo(Path.Combine(path, $"{projectItem?.Name ?? "Unknow"}.xml"));
                if (!Directory.Exists(exportPath.DirectoryName))
                {
                    Directory.CreateDirectory(exportPath.DirectoryName);
                }

                if (File.Exists(exportPath.FullName))
                {
                    File.Delete(exportPath.FullName);
                }

                switch (projectItem)
                {
                    case Models.ProjectTree.Plc.Blocks.Object plcBlock:
                        if(plcBlock.PlcBlock?.IsConsistent == true)
                        { 
                            plcBlock.PlcBlock?.Export(exportPath, ExportOptions.WithDefaults, DocumentInfoOptions.None);
                            exportedPaths.Add(exportPath.FullName);
                        }
                        else
                        {
                            throw new InvalidOperationException($"The block {plcBlock.Name} is not consistent and cannot be exported.");
                        }
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
                    case Models.ProjectTree.Plc.Type plcType:
                        plcType.PlcType?.Export(exportPath, ExportOptions.WithDefaults, DocumentInfoOptions.None);
                        exportedPaths.Add(exportPath.FullName);
                        break;
                    case Models.ProjectTree.Plc.Tag plcTag:
                        plcTag.TagTable?.Export(exportPath, ExportOptions.WithDefaults, DocumentInfoOptions.None);
                        exportedPaths.Add(exportPath.FullName);
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

    public Task<string[]?> ExportAsync(IEngineeringObject projectItem, string? path = null)
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
                DirectoryInfo directoryExport;
                switch (projectItem)
                {
                    case HmiTagTable hmiTagTable:
                        directoryExport = new DirectoryInfo(Path.Combine(path, hmiTagTable.Name));
                        if (Directory.Exists(directoryExport.FullName))
                        {
                            Directory.Delete(directoryExport.FullName, true);
                        }

                        var export = hmiTagTable.Tags.Export(directoryExport);
                        exportedPaths.AddRange(export.Select(s => s.FullName));


                        var yamlConverter = new Converters.YAMLConverter();

                        var data = yamlConverter.Read(exportedPaths[0] + ".hmi.yml");
                        var defaultValues = (yamlConverter.Read(exportedPaths[0] + ".hmi.yml")[0] as Dictionary<object, object>).FirstOrDefault().Value as List<object>;

                        break;
                    case HmiDiscreteAlarm hmiDiscreteAlarm:
                        directoryExport = new DirectoryInfo(Path.Combine(path, hmiDiscreteAlarm.Name));
                        if (Directory.Exists(directoryExport.FullName))
                        {
                            Directory.Delete(directoryExport.FullName, true);
                        }

                        

                        break;

                        //case Models.ProjectTree.Plc.Item plcItem:
                        //    if (plcItem.Items?.Count > 0)
                        //    {
                        //        foreach (var item in plcItem.Items)
                        //        {
                        //            exportedPaths.AddRange(ExportAsync(item, path).Result);
                        //        }
                        //    }
                        //    break;
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

    public Task<Models.ProjectTree.Object?> GetItem(string blockName)
    {
        var tcs = new TaskCompletionSource<Models.ProjectTree.Object?>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if (projectsTreeItems is null)
                {
                    tcs.SetResult(null);
                }
                else
                {
                    var item = GetItemTree<Models.ProjectTree.Plc.Object>(projectsTreeItems, blockName).FirstOrDefault();
                    tcs.SetResult(item);
                }
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    private Models.ProjectTree.Object? FindItemInProjectDevices(DeviceComposition devices, string parentPath, string blockName)
    {
        Models.ProjectTree.Object? deviceItems = null;
        if (devices?.Count > 0)
        {
            foreach (var device in devices)
            {
                foreach (var deviceItem in device.DeviceItems)
                {
                    switch (deviceItem.GetService<SoftwareContainer>()?.Software)
                    {
                        case PlcSoftware plcSoftware:
                            PlcHelper.FindPlcBlocks(plcSoftware, blockName, Path.Combine(parentPath, device.Name));
                            PlcHelper.FindPlcTypes(plcSoftware, blockName, Path.Combine(parentPath, device.Name));
                            PlcHelper.FindPlcTags(plcSoftware, blockName, Path.Combine(parentPath, device.Name));
                            break;
                        case HmiTarget hmiTarget:

                            break;
                        case HmiSoftware hmiSoftware:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return deviceItems;
    }

    private List<Models.ProjectTree.Object>? FindItemInProjectDeviceGroups(DeviceUserGroupComposition devicesGroups, string parentPath, string blockName)
    {
        List<Models.ProjectTree.Object>? groups = null;

        if (devicesGroups?.Count > 0)
        {
            var deviceGroup = devicesGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;

                if (FindItemInProjectDevices(deviceGroup[0].Devices, Path.Combine(groupPath, deviceGroup[0].Name), blockName) is Models.ProjectTree.Object devices)
                {
                    
                }

                if (deviceGroup[0].Groups?.Count > 0 &&
                    FindItemInProjectDeviceGroups(deviceGroup[0].Groups, Path.Combine(groupPath, deviceGroup[0].Name), blockName) is List<Models.ProjectTree.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
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







    private static void GetDeviceList(ref List<Siemens.Engineering.HW.Device> devices, Siemens.Engineering.Project project)
    {
        // First return devices from the root folder
        foreach (Siemens.Engineering.HW.Device device in project.Devices)
        {
            devices.Add(device);
        }

        // then get devices from ungrouped devices
        foreach (Siemens.Engineering.HW.Device device in project.UngroupedDevicesGroup.Devices)
        {
            devices.Add(device);
        }

        // Last crawl trough all devicegroups and their sub groups
        foreach (Siemens.Engineering.HW.DeviceUserGroup devicegroup in project.DeviceGroups)
        {
            recurciveGetDeviceList(ref devices, devicegroup);
        }
    }

    private static void recurciveGetDeviceList(ref List<Siemens.Engineering.HW.Device> devices,
    Siemens.Engineering.HW.DeviceUserGroup devicegroup)
    {
        foreach (Siemens.Engineering.HW.Device device in devicegroup.Devices)
        {
            devices.Add(device);
        }
        foreach (Siemens.Engineering.HW.DeviceUserGroup devicesubgroup in devicegroup.Groups)
        {
            recurciveGetDeviceList(ref devices, devicesubgroup);
        }
    }

    //private void export()
    //{
    //    using (var unifiedData = new UnifiedOpennessConnector("V19", args, new List<CmdArgument>() { new CmdArgument()
    //        {
    //            Default = "", Required = false, OptionToSet = "DefinedAttributes", OptionLong = "--definedattributes", OptionShort = "-da", HelpText = "If you want to export only defined attributes, add a list seperated by semicolon, e.g. Left;Top;Authorization"
    //        } }, "ExcelExporter"))
    //    {
    //        Program.unifiedData = unifiedData;
    //        Work();
    //    }
    //    unifiedData.Log("Export finished");
    //}

    //static void Work()
    //{
    //    bool setProperties = false;
    //    List<string> definedAttributes = null;
    //    if (!string.IsNullOrEmpty(unifiedData.CmdArgs["DefinedAttributes"]))
    //    {
    //        definedAttributes = unifiedData.CmdArgs["DefinedAttributes"].Split(';').ToList();
    //        setProperties = true;
    //    }

    //    var converter = new Converters.YAMLConverter();
    //    var defaultValues = (converter.Read(Directory.GetCurrentDirectory() + "\\DefaultScreen.yml")[0] as Dictionary<object, object>).FirstOrDefault().Value as List<object>;

    //    Dictionary<string, List<Dictionary<string, object>>> exportedValues = new Dictionary<string, List<Dictionary<string, object>>>();

    //    Screen screen = new Screen(definedAttributes);
    //    exportedValues = screen.Export(unifiedData.Screens);
    //    CreateExport(exportedValues, defaultValues, setProperties);
    //    unifiedData.Log("Export done");
    //}

    //private void CreateExport(Dictionary<string, List<Dictionary<string, object>>> exportedValues, List<object> defaultValues, bool setProperties)
    //{
    //    foreach (var screenDict in exportedValues)
    //    {

    //        var differences = GetDifferences(screenDict.Value, defaultValues, setProperties);

    //        string filename = Directory.GetCurrentDirectory() + "\\" + screenDict.Key + ".xlsx";
    //        Microsoft.Office.Interop.Excel.Application xlApp = null;
    //        Microsoft.Office.Interop.Excel.Workbook workbook = null;
    //        Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

    //        try
    //        {
    //            xlApp = new Microsoft.Office.Interop.Excel.Application();
    //            workbook = xlApp.Workbooks.Add();
    //            worksheet = workbook.Worksheets[1];

    //            writeToCells(ref worksheet, differences);

    //            if (File.Exists(filename))
    //            {
    //                File.Delete(filename);
    //            }
    //            object misValue = System.Reflection.Missing.Value;
    //            workbook.SaveAs(filename, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
    //            misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);


    //            workbook.Close(true, misValue, misValue);
    //            xlApp.Quit();
    //        }
    //        finally
    //        {
    //            Marshal.ReleaseComObject(xlApp);
    //            Marshal.ReleaseComObject(workbook);
    //            Marshal.ReleaseComObject(worksheet);
    //            xlApp = null;
    //            workbook = null;
    //            worksheet = null;
    //        }
    //    }
    //}

    //private void writeToCells(ref Worksheet worksheet, List<Dictionary<string, object>> differences)
    //{
    //    worksheet.Cells[1, 1] = "Type";
    //    worksheet.Cells[1, 2] = "Name";
    //    Dictionary<string, int> columnContentToIndex = new Dictionary<string, int>();
    //    columnContentToIndex.Add("Type", 1);
    //    columnContentToIndex.Add("Name", 2);
    //    for (int i = 0; i < differences.Count; i++)
    //    {
    //        writePropertiesToCells(ref worksheet, i + 2, differences[i], ref columnContentToIndex, "");
    //    }
    //}

    //private static void writePropertiesToCells(ref Worksheet worksheet, int rowIndex, Dictionary<string, object> properties, ref Dictionary<string, int> columnContentToIndex, string parentName)
    //{
    //    foreach (var item in properties)
    //    {
    //        if (item.Value is Dictionary<string, object>)
    //        {
    //            writePropertiesToCells(ref worksheet, rowIndex, item.Value as Dictionary<string, object>, ref columnContentToIndex, parentName + item.Key + ".");
    //        }
    //        else if (item.Value is List<object>)
    //        {
    //            var list = (item.Value as List<object>);
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                writePropertiesToCells(ref worksheet, rowIndex, list[i] as Dictionary<string, object>, ref columnContentToIndex, parentName + item.Key + "[" + i + "].");
    //            }
    //        }
    //        else if (item.Value is ValueConverter)
    //        {
    //            // TODO: implement to unpack the values. ValueConverter can be Range, Bitmask, Singlebit
    //        }
    //        else
    //        {
    //            int columnIndex = -1;
    //            if (!columnContentToIndex.TryGetValue(parentName + item.Key, out columnIndex))
    //            {
    //                columnIndex = columnContentToIndex.Count + 1; // worksheet starts at index 1
    //                columnContentToIndex.Add(parentName + item.Key, columnIndex);
    //                worksheet.Cells[1, columnIndex] = parentName + item.Key;
    //            }
    //            var value = item.Value;
    //            if (item.Value is List<string>)
    //            {
    //                value = string.Join(",", item.Value as List<string>);
    //            }
    //            else if (value is bool)
    //            {
    //                // add a space at the end to make sure the english words for true and false will be used. Otherwise Excel will use the local language and it cannot be imported on another PC with different language.
    //                value = ((bool)value == true) ? "True " : "False ";
    //            }
    //            worksheet.Cells[rowIndex, columnIndex] = value;
    //        }
    //    }
    //}

    //private static List<Dictionary<string, object>> GetDifferences(List<Dictionary<string, object>> screenItems, List<object> defaultValues, bool setProperties)
    //{
    //    List<Dictionary<string, object>> differences = new List<Dictionary<string, object>>();
    //    foreach (Dictionary<string, object> attributes in screenItems)
    //    {
    //        string type = attributes["Type"].ToString();
    //        //if (type == "HmiScreen")
    //        //{
    //        //    continue; // screens will not be handled
    //        //}
    //        var defaultObject = defaultValues.Find(x =>
    //        {
    //            object typeName = null;
    //            (x as Dictionary<object, object>).TryGetValue("Type", out typeName);
    //            return typeName.ToString() == type;
    //        }) as Dictionary<object, object>;
    //        if (defaultObject == null)
    //        {
    //            unifiedData.Log("Cannot find default type with name: " + type, LogLevel.Warning);
    //        }
    //        else
    //        {
    //            unifiedData.Log(defaultObject.ToString(), LogLevel.Debug);
    //            var differencesScreenItem = new Dictionary<string, object>();
    //            GetDifferencesScreenItem(attributes, defaultObject, ref differencesScreenItem);
    //            differencesScreenItem.Add("Type", type); // type is always needed to create the object again
    //            if (setProperties)
    //            {
    //                differences.Add(attributes);
    //            }
    //            else
    //            {
    //                differences.Add(differencesScreenItem);
    //            }
    //        }
    //    }
    //    return differences;
    //}

    //private static void GetDifferencesScreenItem(Dictionary<string, object> attributes, Dictionary<object, object> defaultObject, ref Dictionary<string, object> differencesScreenItem)
    //{
    //    foreach (var attribute in attributes)
    //    {
    //        if (attribute.Value is List<object>)
    //        {
    //            var list = attribute.Value as List<object>;
    //            object deeperObj = null;
    //            var deeperList = new List<object>();
    //            if (defaultObject.TryGetValue(attribute.Key, out deeperObj))
    //            {
    //                deeperList = deeperObj as List<object>;
    //            }
    //            var newDiffList = new List<object>();
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                var newDiffValue = new Dictionary<string, object>();
    //                var deeperDict = new Dictionary<object, object>();
    //                if (i < deeperList.Count)
    //                {
    //                    deeperDict = deeperList[i] as Dictionary<object, object>;
    //                }
    //                GetDifferencesScreenItem(list[i] as Dictionary<string, object>, deeperDict, ref newDiffValue);
    //                if (newDiffValue.Count > 0)
    //                {
    //                    newDiffList.Add(newDiffValue);
    //                }
    //            }
    //            if (newDiffList.Count > 0)
    //            {
    //                differencesScreenItem.Add(attribute.Key, newDiffList);
    //            }
    //        }
    //        else if (attribute.Value is Dictionary<string, object>)
    //        {
    //            object deeperObj = null;
    //            var deeperDict = new Dictionary<object, object>();
    //            if (defaultObject.TryGetValue(attribute.Key, out deeperObj))
    //            {
    //                deeperDict = deeperObj as Dictionary<object, object>;
    //            }
    //            var newDiffValue = new Dictionary<string, object>();
    //            GetDifferencesScreenItem(attribute.Value as Dictionary<string, object>, deeperDict, ref newDiffValue);
    //            if (newDiffValue.Count > 0)
    //            {
    //                differencesScreenItem.Add(attribute.Key, newDiffValue);
    //            }
    //        }
    //        else
    //        {
    //            object defaultValue = null;
    //            if (defaultObject.TryGetValue(attribute.Key, out defaultValue) && defaultValue != null)
    //            {
    //                if (string.Compare(attribute.Value.ToString(), defaultValue.ToString(), true) != 0)
    //                {
    //                    differencesScreenItem.Add(attribute.Key, attribute.Value);
    //                }
    //            }
    //            else
    //            {
    //                differencesScreenItem.Add(attribute.Key, attribute.Value);
    //            }
    //        }
    //    }
    //}
}
