using System.Collections.ObjectModel;

using Siemens.Engineering.HW;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;

namespace TiaPortalOpenness.Helpers;

public static class PlcHelper
{
    public static List<Models.ProjectTree.Plc.Object> GetPlcBlocks(PlcSoftware plcSoftware, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var blocks = new List<Models.ProjectTree.Plc.Object>();

        if (GetPlcBlockGroups(plcSoftware.BlockGroup.Groups, parentPath) is List<Models.ProjectTree.Plc.Object> plcBlockGroups)
        {
            blocks.AddRange(plcBlockGroups);
        }
        if (GetPlcBlocks(plcSoftware.BlockGroup.Blocks, parentPath) is List<Models.ProjectTree.Plc.Blocks.Object> plcBlocks)
        {
            blocks.AddRange(plcBlocks);
        }

        return blocks;
    }

    private static List<Models.ProjectTree.Plc.Object>? GetPlcBlockGroups(PlcBlockUserGroupComposition plcBlockGroups, string? parentPath)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcBlockGroups?.Count > 0)
        {
            var blockGroup = plcBlockGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (GetPlcBlocks(blockGroup[0].Blocks, Path.Combine(groupPath, blockGroup[0].Name)) is List<Models.ProjectTree.Plc.Blocks.Object> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (blockGroup[0].Groups?.Count > 0 &&
                    GetPlcBlockGroups(blockGroup[0].Groups, Path.Combine(groupPath, blockGroup[0].Name)) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(blockGroup[0].Name, Path.Combine(groupPath, blockGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                blockGroup.RemoveAt(0);
            } while (blockGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Blocks.Object>? GetPlcBlocks(PlcBlockComposition plcBlocks, string? parentPath)
    {
        List<Models.ProjectTree.Plc.Blocks.Object>? blocks = null;
        if (plcBlocks?.Count() > 0)
        {
            foreach (var block in plcBlocks)
            {
                blocks ??= [];
                switch (block)
                {
                    case OB oB:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.Ob(oB, parentPath));
                        break;
                    case FC fC:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.Fc(fC, parentPath));
                        break;
                    case FB fB:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.Fb(fB, parentPath));
                        break;
                    case ArrayDB arrayDB:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.DataBlocks.ArrayDb(arrayDB, parentPath));
                        break;
                    case InstanceDB instanceDB:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.DataBlocks.InstanceDb(instanceDB, parentPath));
                        break;
                    case GlobalDB globalDB:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.DataBlocks.GlobalData(globalDB, parentPath));
                        break;
                    //case DataBlock dataBlock:
                    //    blocks.Add(new ProjectTree.Plc.Blocks.DataBlocks.DataBlock(dataBlock, parentPath));
                    //    break;
                    case PlcBlock:
                        blocks.Add(new Models.ProjectTree.Plc.Blocks.Item(block, parentPath));
                        break;
                    default:
                        //blocks.Add(new ProjectTree.Plc.Blocks.Object(block.Name, parentPath));
                        break;
                }
            }
        }
        return blocks;
    }



    public static List<Models.ProjectTree.Plc.Object> GetPlcTypes(PlcSoftware plcSoftware, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var typeItems = new List<Models.ProjectTree.Plc.Object>();
        if (GetPlcTypesGroups(plcSoftware.TypeGroup.Groups, parentPath) is List<Models.ProjectTree.Plc.Object> plcTypeGroups)
        {
            typeItems.AddRange(plcTypeGroups);
        }
        if (GetPlcTypes(plcSoftware.TypeGroup.Types, parentPath) is List<Models.ProjectTree.Plc.Type> plcTypes)
        {
            typeItems.AddRange(plcTypes);
        }

        return typeItems;
    }

    private static List<Models.ProjectTree.Plc.Object>? GetPlcTypesGroups(PlcTypeUserGroupComposition plcTypesGroups, string parentPath)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcTypesGroups?.Count > 0)
        {
            var typeGroup = plcTypesGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (GetPlcTypes(typeGroup[0].Types, Path.Combine(groupPath, typeGroup[0].Name)) is List<Models.ProjectTree.Plc.Type> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (typeGroup[0].Groups?.Count > 0 &&
                    GetPlcTypesGroups(typeGroup[0].Groups, Path.Combine(groupPath, typeGroup[0].Name)) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(typeGroup[0].Name, Path.Combine(groupPath, typeGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                typeGroup.RemoveAt(0);
            } while (typeGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Type>? GetPlcTypes(PlcTypeComposition plcTypes, string parentPath)
    {
        List<Models.ProjectTree.Plc.Type>? types = null;
        if (plcTypes?.Count() > 0)
        {
            foreach (var type in plcTypes)
            {
                types ??= [];
                types.Add(new Models.ProjectTree.Plc.Type(type, parentPath));
            }
        }
        return types;
    }



    public static List<Models.ProjectTree.Plc.Object>? GetPlcTags(PlcSoftware plcSoftware, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var tags = new List<Models.ProjectTree.Plc.Object>();
        if (GetPlcTagsGroups(plcSoftware.TagTableGroup.Groups, parentPath) is List<Models.ProjectTree.Plc.Object> plcTagsGroups)
        {
            tags.AddRange(plcTagsGroups);
        }
        if (GetPlcTags(plcSoftware.TagTableGroup.TagTables, parentPath) is List<Models.ProjectTree.Plc.Tag> plcTags)
        {
            tags.AddRange(plcTags);
        }
        return tags;
    }

    private static List<Models.ProjectTree.Plc.Object>? GetPlcTagsGroups(PlcTagTableUserGroupComposition plcTagsGroups, string parentPath)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcTagsGroups?.Count > 0)
        {
            var tagGroup = plcTagsGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (GetPlcTags(tagGroup[0].TagTables, Path.Combine(groupPath, tagGroup[0].Name)) is List<Models.ProjectTree.Plc.Tag> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (tagGroup[0].Groups?.Count > 0 &&
                    GetPlcTagsGroups(tagGroup[0].Groups, Path.Combine(groupPath, tagGroup[0].Name)) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(tagGroup[0].Name, Path.Combine(groupPath, tagGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                tagGroup.RemoveAt(0);
            } while (tagGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Tag>? GetPlcTags(PlcTagTableComposition plcTags, string parentPath)
    {
        List<Models.ProjectTree.Plc.Tag>? tags = null;
        if (plcTags?.Count() > 0)
        {
            foreach (var tag in plcTags)
            {
                tags ??= [];
                tags.Add(new Models.ProjectTree.Plc.Tag(tag, parentPath));
            }
        }
        return tags;
    }



    public static List<Models.ProjectTree.Plc.Blocks.Object> FindPlcBlocks(PlcSoftware plcSoftware, string blockName, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var blocks = new List<Models.ProjectTree.Plc.Blocks.Object>();

        if (FindPlcBlockGroups(plcSoftware.BlockGroup.Groups, parentPath, blockName) is List<Models.ProjectTree.Plc.Object> plcBlockGroups)
        {
            blocks.AddRange(plcBlockGroups);
        }
        if (FindPlcBlocks(plcSoftware.BlockGroup.Blocks, parentPath, blockName) is List<Models.ProjectTree.Plc.Blocks.Object> plcBlocks)
        {
            blocks.AddRange(plcBlocks);
        }

        return blocks;
    }

    private static List<Models.ProjectTree.Plc.Object>? FindPlcBlockGroups(PlcBlockUserGroupComposition plcBlockGroups, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcBlockGroups?.Count > 0)
        {
            var blockGroup = plcBlockGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (FindPlcBlocks(blockGroup[0].Blocks, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Blocks.Object> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (blockGroup[0].Groups?.Count > 0 &&
                    FindPlcBlockGroups(blockGroup[0].Groups, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(blockGroup[0].Name, Path.Combine(groupPath, blockGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                blockGroup.RemoveAt(0);
            } while (blockGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Blocks.Object> FindPlcBlocks(PlcBlockComposition plcBlocks, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Blocks.Object>? blocks = null;
        var block = plcBlocks?.Find(blockName);
        return blocks;
    }

    public static List<Models.ProjectTree.Plc.Type> FindPlcTypes(PlcSoftware plcSoftware, string blockName, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var blocks = new List<Models.ProjectTree.Plc.Type>();

        if (FindPlcTypesGroups(plcSoftware.TypeGroup.Groups, parentPath, blockName) is List<Models.ProjectTree.Plc.Object> plcBlockGroups)
        {
            blocks.AddRange(plcBlockGroups);
        }
        if (FindPlcTypes(plcSoftware.TypeGroup.Types, parentPath, blockName) is List<Models.ProjectTree.Plc.Type> plcBlocks)
        {
            blocks.AddRange(plcBlocks);
        }

        return blocks;
    }

    private static List<Models.ProjectTree.Plc.Object>? FindPlcTypesGroups(PlcTypeUserGroupComposition plcBlockGroups, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcBlockGroups?.Count > 0)
        {
            var blockGroup = plcBlockGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (FindPlcTypes(blockGroup[0].Types, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Type> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (blockGroup[0].Groups?.Count > 0 &&
                    FindPlcTypesGroups(blockGroup[0].Groups, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(blockGroup[0].Name, Path.Combine(groupPath, blockGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                blockGroup.RemoveAt(0);
            } while (blockGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Type> FindPlcTypes(PlcTypeComposition plcBlocks, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Type>? blocks = null;
        var block = plcBlocks?.Find(blockName);
        return blocks;
    }

    public static List<Models.ProjectTree.Plc.Tag> FindPlcTags(PlcSoftware plcSoftware, string blockName, string? parentPath = null)
    {
        if (plcSoftware is null) throw new ArgumentNullException(nameof(plcSoftware));
        var blocks = new List<Models.ProjectTree.Plc.Tag>();

        if (FindPlcTagsGroups(plcSoftware.TagTableGroup.Groups, parentPath, blockName) is List<Models.ProjectTree.Plc.Object> plcBlockGroups)
        {
            blocks.AddRange(plcBlockGroups);
        }
        if (FindPlcTags(plcSoftware.TagTableGroup.TagTables, parentPath, blockName) is List<Models.ProjectTree.Plc.Tag> plcBlocks)
        {
            blocks.AddRange(plcBlocks);
        }

        return blocks;
    }

    private static List<Models.ProjectTree.Plc.Object>? FindPlcTagsGroups(PlcTagTableUserGroupComposition plcBlockGroups, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Object>? groups = null;

        if (plcBlockGroups?.Count > 0)
        {
            var blockGroup = plcBlockGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<Models.ProjectTree.Object>? groupItems = null;
                if (FindPlcTags(blockGroup[0].TagTables, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Tag> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (blockGroup[0].Groups?.Count > 0 &&
                    FindPlcTagsGroups(blockGroup[0].Groups, Path.Combine(groupPath, blockGroup[0].Name), blockName) is List<Models.ProjectTree.Plc.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new Models.ProjectTree.Plc.Item(blockGroup[0].Name, Path.Combine(groupPath, blockGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                blockGroup.RemoveAt(0);
            } while (blockGroup.Count > 0);
        }

        return groups;
    }

    private static List<Models.ProjectTree.Plc.Tag> FindPlcTags(PlcTagTableComposition plcBlocks, string? parentPath, string blockName)
    {
        List<Models.ProjectTree.Plc.Tag>? blocks = null;
        var block = plcBlocks?.Find(blockName);
        return blocks;
    }
}
