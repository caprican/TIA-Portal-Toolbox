using System.Collections.ObjectModel;

using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public class Plc : Devices.Object
{
    internal PlcSoftware Device;

    public new ObservableCollection<ProjectTree.Plc.Object>? Items { get; private set; }

    public Plc(PlcSoftware plc, string parentPath) : base(plc.Name, parentPath)
    {
        Device = plc;

        var blockItems = new ObservableCollection<ProjectTree.Plc.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            if (GetPlcBlockGroups(Device.BlockGroup.Groups, parentPath) is List<ProjectTree.Plc.Object> plcBlockGroups)
            {
                blockItems.AddRange(plcBlockGroups);
            }
            if (GetPlcBlocks(Device.BlockGroup.Blocks, parentPath) is List<ProjectTree.Plc.Blocks.Object> plcBlocks)
            {
                blockItems.AddRange(plcBlocks);
            }
        }
        if (blockItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.BlockGroup.Name, parentPath) { Items = blockItems });
        }

        var tagItems = new ObservableCollection<ProjectTree.Plc.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            if (GetPlcTagsGroups(Device.TagTableGroup.Groups, parentPath) is List<ProjectTree.Plc.Object> plcTagGroups)
            {
                tagItems.AddRange(plcTagGroups);
            }
            if (GetPlcTags(Device.TagTableGroup.TagTables, parentPath) is List<ProjectTree.Plc.Tag> plcTags)
            {
                tagItems.AddRange(plcTags);
            }
        }
        if (tagItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.TagTableGroup.Name, parentPath) { Items = tagItems });
        }

        var typeItems = new ObservableCollection<ProjectTree.Plc.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            if (GetPlcTypesGroups(Device.TypeGroup.Groups, parentPath) is List<ProjectTree.Plc.Object> plcTypeGroups)
            {
                typeItems.AddRange(plcTypeGroups);
            }
            if (GetPlcTypes(Device.TypeGroup.Types, parentPath) is List<ProjectTree.Plc.Type> plcTypes)
            {
                typeItems.AddRange(plcTypes);
            }
        }
        if (typeItems?.Count > 0)
        {
            Items ??= [];
            Items.Add(new ProjectTree.Plc.Item(Device.TypeGroup.Name, parentPath) { Items = typeItems });
        }
    }

    private List<ProjectTree.Plc.Object>? GetPlcBlockGroups(PlcBlockUserGroupComposition plcBlockGroups, string parentPath)
    {
        List<ProjectTree.Plc.Object>? groups = null;

        if (plcBlockGroups?.Count > 0)
        {
            var blockGroup = plcBlockGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<ProjectTree.Plc.Object>? groupItems = null;
                if (GetPlcBlocks(blockGroup[0].Blocks, System.IO.Path.Combine(groupPath, blockGroup[0].Name)) is List<ProjectTree.Plc.Blocks.Object> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (blockGroup[0].Groups?.Count > 0 &&
                    GetPlcBlockGroups(blockGroup[0].Groups, System.IO.Path.Combine(groupPath, blockGroup[0].Name)) is List<ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new ProjectTree.Plc.Item(blockGroup[0].Name, System.IO.Path.Combine(groupPath, blockGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                blockGroup.RemoveAt(0);
            } while (blockGroup.Count > 0);
        }

        return groups;
    }

    private List<ProjectTree.Plc.Blocks.Object>? GetPlcBlocks(PlcBlockComposition plcBlocks, string parentPath)
    {
        List<ProjectTree.Plc.Blocks.Object>? blocks = null;
        if (plcBlocks?.Count() > 0)
        {
            foreach (var block in plcBlocks)
            {
                blocks ??= [];
                switch (block)
                {
                    case OB oB:
                        blocks.Add(new ProjectTree.Plc.Blocks.Ob(oB, parentPath));
                        break;
                    case FC fC:
                        blocks.Add(new ProjectTree.Plc.Blocks.Fc(fC, parentPath));
                        break;
                    case FB fB:
                        blocks.Add(new ProjectTree.Plc.Blocks.Fb(fB, parentPath));
                        break;
                    case ArrayDB arrayDB:
                        blocks.Add(new ProjectTree.Plc.Blocks.DataBlocks.ArrayDb(arrayDB, parentPath));
                        break;
                    case InstanceDB instanceDB:
                        blocks.Add(new ProjectTree.Plc.Blocks.DataBlocks.InstanceDb(instanceDB, parentPath));
                        break;
                    case GlobalDB globalDB:
                        blocks.Add(new ProjectTree.Plc.Blocks.DataBlocks.GlobalData(globalDB, parentPath));
                        break;
                    //case DataBlock dataBlock:
                    //    blocks.Add(new ProjectTree.Plc.Blocks.DataBlocks.DataBlock(dataBlock, parentPath));
                    //    break;
                    case PlcBlock:
                        blocks.Add(new ProjectTree.Plc.Blocks.Item(block, parentPath));
                        break;
                    default:
                        //blocks.Add(new ProjectTree.Plc.Blocks.Object(block.Name, parentPath));
                        break;
                }
            }
        }
        return blocks;
    }

    private List<ProjectTree.Plc.Object>? GetPlcTypesGroups(PlcTypeUserGroupComposition plcTypesGroups, string parentPath)
    {
        List<ProjectTree.Plc.Object>? groups = null;

        if (plcTypesGroups?.Count > 0)
        {
            var typeGroup = plcTypesGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<ProjectTree.Plc.Object>? groupItems = null;
                if (GetPlcTypes(typeGroup[0].Types, System.IO.Path.Combine(groupPath, typeGroup[0].Name)) is List<ProjectTree.Plc.Type> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (typeGroup[0].Groups?.Count > 0 &&
                    GetPlcTypesGroups(typeGroup[0].Groups, System.IO.Path.Combine(groupPath, typeGroup[0].Name)) is List<ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new ProjectTree.Plc.Item(typeGroup[0].Name, System.IO.Path.Combine(groupPath, typeGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                typeGroup.RemoveAt(0);
            } while (typeGroup.Count > 0);
        }

        return groups;
    }

    private List<ProjectTree.Plc.Type>? GetPlcTypes(PlcTypeComposition plcTypes, string parentPath)
    {
        List<ProjectTree.Plc.Type>? types = null;
        if (plcTypes?.Count() > 0)
        {
            foreach (var type in plcTypes)
            {
                types ??= [];
                types.Add(new ProjectTree.Plc.Type(type, parentPath));
            }
        }
        return types;
    }

    private List<ProjectTree.Plc.Object>? GetPlcTagsGroups(PlcTagTableUserGroupComposition plcTagsGroups, string parentPath)
    {
        List<ProjectTree.Plc.Object>? groups = null;

        if (plcTagsGroups?.Count > 0)
        {
            var tagGroup = plcTagsGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<ProjectTree.Plc.Object>? groupItems = null;
                if (GetPlcTags(tagGroup[0].TagTables, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Plc.Tag> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (tagGroup[0].Groups?.Count > 0 &&
                    GetPlcTagsGroups(tagGroup[0].Groups, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new ProjectTree.Plc.Item(tagGroup[0].Name, System.IO.Path.Combine(groupPath, tagGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                tagGroup.RemoveAt(0);
            } while (tagGroup.Count > 0);
        }

        return groups;
    }

    private List<ProjectTree.Plc.Tag>? GetPlcTags(PlcTagTableComposition plcTags, string parentPath)
    {
        List<ProjectTree.Plc.Tag>? tags = null;
        if (plcTags?.Count() > 0)
        {
            foreach (var tag in plcTags)
            {
                tags ??= [];
                tags.Add(new ProjectTree.Plc.Tag(tag, parentPath));
            }
        }
        return tags;
    }

    public override Task<bool> ExportAsync(string path)
    {
        throw new NotImplementedException();
    }
}
