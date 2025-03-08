using System.Collections.ObjectModel;

using Siemens.Engineering.SW;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public class Plc : Devices.Object
{
    internal PlcSoftware Device;

    public Plc(PlcSoftware plc, string parentPath) : base(plc.Name, parentPath)
    {
        Device = plc;

        var blockItems = new ObservableCollection<ProjectTree.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            blockItems.AddRange(Helpers.PlcHelper.GetPlcBlocks(Device, parentPath));
        }
        if (blockItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.BlockGroup.Name, parentPath) { Items = blockItems });
        }

        var tagItems = new ObservableCollection<ProjectTree.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            tagItems.AddRange(Helpers.PlcHelper.GetPlcTags(Device, parentPath));
        }
        if (tagItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.TagTableGroup.Name, parentPath) { Items = tagItems });
        }

        var typeItems = new ObservableCollection<ProjectTree.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            typeItems.AddRange(Helpers.PlcHelper.GetPlcTypes(Device, parentPath));
        }
        if (typeItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.TypeGroup.Name, parentPath) { Items = typeItems });
        }
    }
}
