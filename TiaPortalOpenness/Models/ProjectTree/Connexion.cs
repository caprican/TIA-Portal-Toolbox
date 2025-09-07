using CommunityToolkit.Mvvm.ComponentModel;

using Siemens.Engineering.Hmi.Communication;
using Siemens.Engineering.HmiUnified.HmiConnections;

using TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.DataBlocks;

namespace TiaPortalOpenness.Models.ProjectTree;

public class Connexion : ObservableObject
{
    internal Devices.Hmi? HmiDevice;
    internal Devices.Unified? UnifiedDevice;
    internal Devices.Plc? PlcDevice;

    public string Name { get; }
    public string HmiName => HmiDevice?.Name ?? UnifiedDevice?.Name ?? string.Empty;
    public string PlcName => PlcDevice?.Name ?? string.Empty;

    public IEnumerable<string>? AlarmClasses => UnifiedDevice?.AlarmClasses ?? HmiDevice?.AlarmClasses ?? [];

    public IEnumerable<DataBlock>? Blocks { get; }

    public List<Models.HmiDataBlockTag>? Tags { get; internal set; }

    public Connexion(Connection connection, Devices.Hmi hmiDevice, Devices.Plc plcDevice, IEnumerable<DataBlock>? block)
    {
        Name = connection.Name;
        PlcDevice = plcDevice;
        HmiDevice = hmiDevice;
        UnifiedDevice = null;

        Blocks = block;
    }

    public Connexion(HmiConnection connection, Devices.Unified unifiedDevice, Devices.Plc plcDevice, IEnumerable<DataBlock>? block)
    {
        Name = connection.Name;
        PlcDevice = plcDevice;
        HmiDevice = null;
        UnifiedDevice = unifiedDevice;

        Blocks = block;
    }
}
