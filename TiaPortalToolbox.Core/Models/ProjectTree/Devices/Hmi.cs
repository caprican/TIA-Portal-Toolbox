using System.Collections.ObjectModel;

using Siemens.Engineering.Hmi;
using Siemens.Engineering.Hmi.Tag;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Devices;

public class Hmi : Devices.Object
{
    internal HmiTarget Device;

    public new ObservableCollection<ProjectTree.Hmi.Object>? Items =>
    [
        new ProjectTree.Hmi.Item(Device.TagFolder.Name, Path) { Items = Tags }
    ];

    public ObservableCollection<ProjectTree.Hmi.Object>? Tags { get; set; }//=> Items.SingleOrDefault(s => s.Name == "HMI Tags")?.Items;

    public IEnumerable<string>? AlarmClasses => null;

    public Hmi(HmiTarget hmi, string? parentPath) : base(hmi.Name, parentPath)
    {
        Device = hmi;

        var tagItems = new ObservableCollection<ProjectTree.Hmi.Object>();
        if (!string.IsNullOrEmpty(parentPath))
        {
            if (GetHmiTagsGroups(hmi.TagFolder.Folders, parentPath!) is List<ProjectTree.Hmi.Item> plcTagGroups)
            {
                tagItems.AddRange(plcTagGroups);
            }
            if (GetHmiTags(Device.TagFolder.TagTables, parentPath!) is List<ProjectTree.Hmi.TagTable> plcTags)
            {
                tagItems.AddRange(plcTags);
            }
        }

        if (tagItems?.Count > 0)
        {
            //Items ??= [];
            //Items.Add(new Models.ProjectTreeHmiItem(Hmi.TagFolder.Name, parentPath) { Items = new ObservableCollection<Models.ProjectTreeHmiItem>(tagItems.OrderBy(o => o.Name)) });
            Tags = tagItems;
        }
    }

    private List<ProjectTree.Hmi.Item>? GetHmiTagsGroups(TagUserFolderComposition plcTagsGroups, string parentPath)
    {
        List<ProjectTree.Hmi.Item>? groups = null;

        if (plcTagsGroups?.Count > 0)
        {
            var tagGroup = plcTagsGroups.ToList();
            var groupPath = parentPath;
            do
            {
                ObservableCollection<ProjectTree.Hmi.Object>? groupItems = null;
                if (GetHmiTags(tagGroup[0].TagTables, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Hmi.TagTable> blocks)
                {
                    groupItems ??= [];
                    groupItems.AddRange(blocks);
                }

                if (tagGroup[0].Folders?.Count > 0 &&
                    GetHmiTagsGroups(tagGroup[0].Folders, System.IO.Path.Combine(groupPath, tagGroup[0].Name)) is List<ProjectTree.Hmi.Item> lstGroup)
                {
                    groupItems ??= [];
                    groupItems.AddRange(lstGroup);
                }

                if (groupItems is not null)
                {
                    groups ??= [];
                    groups.Add(new ProjectTree.Hmi.Item(tagGroup[0].Name, System.IO.Path.Combine(groupPath, tagGroup[0].Name))
                    {
                        Items = groupItems
                    });
                }
                tagGroup.RemoveAt(0);
            } while (tagGroup.Count > 0);
        }

        return groups;
    }

    private List<ProjectTree.Hmi.TagTable>? GetHmiTags(TagTableComposition plcTags, string parentPath)
    {
        List<ProjectTree.Hmi.TagTable>? tags = null;
        if (plcTags?.Count() > 0)
        {
            foreach (var tag in plcTags)
            {
                tags ??= [];
                tags.Add(new ProjectTree.Hmi.TagTable(tag, parentPath));
            }
        }
        return tags;
    }

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
