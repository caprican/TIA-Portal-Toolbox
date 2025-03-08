using System.Collections.ObjectModel;

using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HmiUnified.HmiTags;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public class Unified : Devices.Object
{
    internal HmiSoftware Device;
    //public new ObservableCollection<ProjectTree.Unified.Item>? Items =>
    //[
    //    //new ProjectTree.Unified.Item("HMI Tags", Path) { Items = Tags }
    //];

    public ObservableCollection<ProjectTree.Unified.Object>? Tags { get; set; }//=> Items.SingleOrDefault(s => s.Name == "HMI Tags")?.Items;

    public IEnumerable<string>? AlarmClasses => Device.AlarmClasses.Select(s => s.Name);

    public Unified(HmiSoftware hmi, string? parentPath) : base(hmi.Name, parentPath)
    {
        Device = hmi;

        var tagItems = new ObservableCollection<ProjectTree.Unified.Object>();
        if (GetUnifiedTagsGroups(Device.TagTableGroups, Path ?? string.Empty) is List<ProjectTree.Unified.Object> unifiedTagGroups)
        {
            tagItems.AddRange(unifiedTagGroups);
        }
        if (GetUnifiedTags(Device.TagTables, Path ?? string.Empty) is List<ProjectTree.Unified.TagTable> unifiedTags)
        {
            tagItems.AddRange(unifiedTags);
        }
        if (tagItems?.Count > 0)
        {
            //Items ??= [];
            //Items.Add(new ProjectTreeUnifiedItem("HMI Tags", parentPath) { Items = new ObservableCollection<ProjectTreeUnifiedItem>(tagItems.OrderBy(o => o.Name)) });
            Tags = tagItems;
        }
    }

    private List<ProjectTree.Unified.Object>? GetUnifiedTagsGroups(HmiTagTableGroupComposition plcTagsGroups, string parentPath)
    {
        List<ProjectTree.Unified.Object>? groups = null;

        if (plcTagsGroups?.Count > 0)
        {
            var tagGroup = plcTagsGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<ProjectTree.Object>? groupItems = null;
                if (GetUnifiedTags(tagGroup[0].TagTables, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Unified.TagTable> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (tagGroup[0].Groups?.Count > 0 &&
                    GetUnifiedTagsGroups(tagGroup[0].Groups, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Unified.Object> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new ProjectTree.Unified.Folder(tagGroup[0].Name, System.IO.Path.Combine(groupPath, tagGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                tagGroup.RemoveAt(0);
            } while (tagGroup.Count > 0);
        }

        return groups;
    }

    private List<ProjectTree.Unified.TagTable>? GetUnifiedTags(HmiTagTableComposition plcTags, string parentPath)
    {
        List<ProjectTree.Unified.TagTable>? tags = null;
        if (plcTags?.Count() > 0)
        {
            foreach (var tag in plcTags)
            {
                tags ??= [];
                switch(tag)
                {
                    case HmiTagTable table:
                        tags.Add(new ProjectTree.Unified.TagTable(table, parentPath));
                        break;
                }

            }
        }
        return tags;
    }
}
