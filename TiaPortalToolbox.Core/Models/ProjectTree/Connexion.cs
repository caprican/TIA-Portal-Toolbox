using CommunityToolkit.Mvvm.ComponentModel;

using Siemens.Engineering.Hmi.Communication;
using Siemens.Engineering.HmiUnified.HmiConnections;

namespace TiaPortalToolbox.Core.Models.ProjectTree;

public class Connexion : ObservableObject
{
    internal Devices.Hmi? HmiDevice;
    internal Devices.Unified? UnifiedDevice;
    internal Devices.Plc? PlcDevice;

    public string Name { get; }
    public string HmiName => HmiDevice?.Name ?? UnifiedDevice?.Name ?? string.Empty;
    public string PlcName => PlcDevice?.Name ?? string.Empty;

    public IEnumerable<string>? AlarmClasses => UnifiedDevice?.AlarmClasses ?? HmiDevice?.AlarmClasses ?? [];

    public IEnumerable<Plc.Blocks.DataBlocks.DataBlock>? Blocks { get; }

    public Connexion(Connection connection, Devices.Hmi hmiDevice, Devices.Plc plcDevice, IEnumerable<Plc.Blocks.DataBlocks.DataBlock>? block)
    {
        Name = connection.Name;
        PlcDevice = plcDevice;
        HmiDevice = hmiDevice;
        UnifiedDevice = null;

        Blocks = block;
    }

    public Connexion(HmiConnection connection, Devices.Unified unifiedDevice, Devices.Plc plcDevice, IEnumerable<Plc.Blocks.DataBlocks.DataBlock>? block)
    {
        Name = connection.Name;
        PlcDevice = plcDevice;
        HmiDevice = null;
        UnifiedDevice = unifiedDevice;

        Blocks = block;
    }
}
