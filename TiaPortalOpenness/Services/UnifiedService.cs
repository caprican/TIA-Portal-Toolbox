using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using Siemens.Engineering;
using Siemens.Engineering.Hmi.Tag;
using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HmiUnified.HmiAlarm.HmiAlarmCommon;
using Siemens.Engineering.HmiUnified.HmiTags;
using Siemens.Engineering.HW;

using SimaticML.SW.Common;

namespace TiaPortalOpenness.Services;

public class UnifiedService(Contracts.Services.IOpennessService opennessService) : Contracts.Services.IUnifiedService
{
    private readonly Contracts.Services.IOpennessService opennessService = opennessService;

    private static string UnifiedPlcTagNormalized(string tagname)
    {
        var compose = tagname.Split('.');
        string plcTag = compose.FirstOrDefault();
        foreach (var item in compose.Skip(1))
        {
            if (item.Contains(" "))
                plcTag += $".\"{item}\"";
            else
                plcTag += $".{item}";
        }

        return plcTag;
    }

    public string? GetAlarmsClassName(Models.ProjectTree.Devices.Unified hmiUnified, string classname, string? tagname, string defaultClassname)
    {
        if (hmiUnified.AlarmClasses.Contains(classname))
        {
            return classname;
        }
        else if (hmiUnified.AlarmClasses.Contains(tagname))
        {
            return tagname;
        }
        else
            return defaultClassname;
    }

    public Task BuildHmiTags(IEnumerable<Models.ProjectTree.Connexion> connexions, string? defaultAlarmsClass, bool simplifyTagname)
    {
        var tcs = new TaskCompletionSource<IEnumerable<Models.ProjectTree.Connexion>?>();
        ThreadPool.QueueUserWorkItem(async _ =>
        {
            try
            {
                //(var tagFolders, var tags, var alarms) = ExtractHmiTags(Devices, markBlock, defaultAlarmsClass, simplifyTagname);
                ////foreach (var folder in tagFolders)
                ////{
                ////    tiaPortalService.UnifiedTagsFolder(folder);

                ////    Debug.WriteLine($"{DateTime.Now.ToLocalTime()} : {folder.Name}");
                ////    Log += $"{DateTime.Now.ToLocalTime()} : {folder.Name}\r\n";
                ////}

                //foreach (var hmi in tags.GroupBy(device => device.Hmi))
                //{
                //    tiaPortalService.UnifiedTag(hmi.Key, hmi);

                //    //Debug.WriteLine($"{DateTime.Now.ToLocalTime()} : {tag.Tagname}");
                //    //Log += $"{DateTime.Now.ToLocalTime()} : {tag.Tagname}\r\n";
                //}

                if (connexions?.Count() > 0)
                {
                    ExtractTagAlarms(connexions, defaultAlarmsClass);

                    BuildHmiTagTables(connexions);
                    BuildHmiTags(connexions, simplifyTagname);
                    BuildDiscretAlarms(connexions);

                    //await ExportTagTable(connexions);
                }
                tcs.SetResult(connexions);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    public Task BuildHmiAlarms(IEnumerable<Models.ProjectTree.Connexion> connexions, string? defaultAlarmsClass, bool simplifyTagname)
    {
        var tcs = new TaskCompletionSource<IEnumerable<Models.ProjectTree.Connexion>?>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if (connexions?.Count() > 0)
                {
                    ExtractTagAlarms(connexions, defaultAlarmsClass);
                    //BuildHmiTags(connexions, false);
                    BuildDiscretAlarms(connexions);
                }
                tcs.SetResult(connexions);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;

    }

    private List<Models.InterfaceMember> GetInterfaceMember(SimaticML.SW.InterfaceSections.IMember member, string parent = "")
    {
        var tags = new List<Models.InterfaceMember>();
        var subTags = new List<Models.InterfaceMember>();
        var tag = new Models.InterfaceMember
        {
            Name = string.IsNullOrEmpty(parent) ? member.Name : $"{parent}.{member.Name}",
            Direction = Models.DirectionMember.Static,
            Type = member.Datatype,
            Descriptions = GetMemberComment(member.Comment)
        };

        foreach (var item in member)
        {
            switch (item)
            {
                case SimaticML.SW.InterfaceSections.ISections sectionItem:
                    break;
                case SimaticML.SW.InterfaceSections.IMember memberItem:
                    subTags.AddRange(GetInterfaceMember(memberItem, tag.Name));
                    break;
                case SimaticML.SW.InterfaceSections.IStartValue startValue:
                    break;
                case SimaticML.SW.InterfaceSections.ISubelement subelement:
                    break;
            }
        }
        tags.Add(tag);
        tags.AddRange(subTags);

        return tags;
    }

    private Dictionary<CultureInfo, string>? GetMemberComment(IComment_T comments)
    {
        var result = new Dictionary<CultureInfo, string>();
        if(comments?.Count() > 0)
        {
            foreach (var comment in comments)
            {
                result.Add(CultureInfo.GetCultureInfo(comment.Lang), comment.Value);
            }
        }

        return result.Count > 0 ? result : null;
    }

    private void ExtractTagAlarms(IEnumerable<Models.ProjectTree.Connexion> connexions, string? defaultAlarmsClass)
    {
        foreach (var connexion in connexions)
        {
            if (connexion.Blocks?.Count() > 0 && connexion.PlcDevice is not null && connexion.UnifiedDevice is not null)
            {
                connexion.Tags ??= [];
                foreach (var block in connexion.Blocks)
                {
                    var exportedPaths = opennessService.ExportAsync(block).Result;

                    if(exportedPaths?.Length > 0)
                    {
                        foreach (var exportPath in exportedPaths)
                        {
                            var serializer = new XmlSerializer(typeof(SimaticML.Document));
                            var myFile = new FileStream(exportPath, FileMode.Open);
                            if (serializer.Deserialize(myFile) is SimaticML.Document document)
                            {

                                foreach (var exportedGlobalDataBlock in document.OfType<SimaticML.SW.Blocks.GlobalDB>())
                                {
                                    var exportGlobalDBSections = exportedGlobalDataBlock.Attributes.Interface.FirstOrDefault(sections => sections.Any(section => section.Name == SectionName_TE.Static));
                                    if (exportGlobalDBSections is not null)
                                    {
                                        var section = exportGlobalDBSections.SingleOrDefault(s => s.Name == SectionName_TE.Static);
                                        var alarmsClassName = defaultAlarmsClass;
                                        foreach (var exportedMember in section.Members)
                                        {
                                            foreach (var member in GetInterfaceMember(exportedMember))
                                            {
                                                switch (member.Type)
                                                {
                                                    case "Struct":
                                                        if(opennessService.ProjectLanguages is not null)
                                                        {
                                                            foreach (var lang in opennessService.ProjectLanguages)
                                                            {
                                                                var culture = CultureInfo.GetCultureInfo(lang.Name);
                                                                if (member.Descriptions?.ContainsKey(culture) == true)
                                                                {
                                                                    var txt = member.Descriptions[culture];
                                                                    if (!string.IsNullOrEmpty(txt))
                                                                    {
                                                                        var match = Regex.Match(txt, @"\[AlarmClass=(.+?)\]", RegexOptions.IgnoreCase);
                                                                        alarmsClassName = GetAlarmsClassName(connexion.UnifiedDevice, match.Groups[1].Value, member.Name, defaultAlarmsClass);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    case "Bool":
                                                        var tagmane = UnifiedPlcTagNormalized($"{block.Name}.{member.Name}");
                                                        if (connexion.Tags.SingleOrDefault(tag => tag.FullName == member.Name) is Models.HmiDataBlockTag hmiTag)
                                                        {
                                                            hmiTag.Tagname = tagmane;
                                                            hmiTag.HmiTagname = tagmane.Replace('.', '_');
                                                            hmiTag.ClassAlarm = alarmsClassName;
                                                            hmiTag.Descriptions = member.Descriptions;
                                                        }
                                                        else
                                                        {
                                                            connexion.Tags.Add(new()
                                                            {
                                                                FullName = member.Name,
                                                                DataBlockName = block.Name,
                                                                Tagname = tagmane,
                                                                HmiTagname = tagmane.Replace('.', '_'),
                                                                ClassAlarm = alarmsClassName,
                                                                Descriptions = member.Descriptions,
                                                                FolderName = block.Parent,
                                                            });
                                                        }


                                                            //tags.Add(new Models.ProjectTree.Unified.Tag
                                                            //{
                                                            //    Hmi = connexion.UnifiedDevice,
                                                            //    Connexion = connexion.Name,
                                                            //    PlcTag = $"{(block.Name.Contains(' ') ? $"\"{block.Name}\"" : block.Name)}.{member.Name}",
                                                            //    Tagname = $"{(simplifyTagname ? block.Name.Replace(markBlock, string.Empty) : block.Name)}_{member.Name.Replace(".", "_")}",
                                                            //    Folder = block.Parent
                                                            //});
                                                            //alarms.Add(new TIAOpennessAdapter.Models.UnifiedAlarm
                                                            //{
                                                            //    Hmi = connexion.UnifiedDevice,
                                                            //    ClassName = alarmsClassName,
                                                            //    Tagname = $"{(simplifyTagname ? block.Name.Replace(markBlock, string.Empty) : block.Name)}_{member.Name.Replace(".", "_")}",
                                                            //    Origin = block.Name.Replace(markBlock, string.Empty),
                                                            //    Descriptions = member.Description,
                                                            //});
                                                            break;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void BuildHmiTagTables(IEnumerable<Models.ProjectTree.Connexion> connexions)
    {
        foreach (var connexion in connexions)
        {
            if (connexion.UnifiedDevice is null || connexion.Blocks is null) continue;

            List<string> tagTables = [];
            foreach(var block in connexion.Blocks)
            {
                tagTables.Add(@$"{block.Path.Remove(connexion.PlcName)}");
            }

            if(connexion?.UnifiedDevice?.Tags is not null)
            {
                var tagTableExists =new List<Models.ProjectTree.Unified.Object>( connexion?.UnifiedDevice?.Tags);
                do
                {
                    if (tagTableExists[0] is not Models.ProjectTree.Unified.Folder && tagTableExists?[0].FullPath is string path && tagTables.Contains(path.Remove(connexion!.HmiName)))
                    {
                        tagTables.Remove(path.Remove(connexion!.HmiName));
                    }
                    if (tagTableExists?[0].Items?.Count > 0)
                    {
                        tagTableExists.AddRange(tagTableExists?[0].Items);
                    }
                    tagTableExists?.RemoveAt(0);
                } while (tagTableExists?.Count > 0);
            }

            if(tagTables?.Count > 0)
            {
                foreach(var tagTable in tagTables)
                {
                    var folders = tagTable.Split('\\');
                    var group = connexion?.UnifiedDevice.Device.TagTableGroups;
                    HmiTagTableComposition? tagTablesFinded = connexion?.UnifiedDevice.Device.TagTables;
                    foreach(var item in folders.Skip(1).Take(folders.Length - 2))
                    {
                        if (group?.Find(item) is HmiTagTableGroup g)
                        {
                            group = g.Groups;
                            tagTablesFinded = g.TagTables;
                        }
                        else
                        {
                            group = group?.Create(item).Groups;
                        }
                    }

                    var tableName = folders[folders.Length - 1];
                    if (!tagTablesFinded.Any(table => table.Name == tableName))
                    {
                        tagTablesFinded!.Create(tableName);
                    }
                }
            }
        }
    }

    private void BuildHmiTags(IEnumerable<Models.ProjectTree.Connexion> connexions, bool simplifyTagname)
    {
        foreach (var connexion in connexions)
        {
            if (connexion.UnifiedDevice is null || connexion.Tags is null) continue;

            var hmiTags = connexion.UnifiedDevice.Device.Tags.ToList();
            var discreteAlarms = connexion.UnifiedDevice.Device.DiscreteAlarms.ToList();
            var tags = connexion.Tags.ToList();
            var discreteAlarmsDeleted = new List<string>();

            foreach (var dataBlockTag in connexion.Tags)
            {
                foreach (var hmiTag in hmiTags.Where(unifiedTag => unifiedTag.PlcTag == dataBlockTag.Tagname))
                {
                    var originaltagname = hmiTag.Name;
                    discreteAlarmsDeleted.AddRange(discreteAlarms.FindAll(f => f.RaisedStateTag == originaltagname).Select(s => s.Name));
                    tags.Remove(dataBlockTag);
                }
            }

            foreach (var alarm in discreteAlarmsDeleted)
            {
                connexion.UnifiedDevice.Device.DiscreteAlarms.Find(alarm).Delete();
            }

            foreach (var dataBlockTag in tags)
            {
                var hmiTag = string.IsNullOrEmpty(dataBlockTag.FolderName) ? connexion.UnifiedDevice.Device.Tags.Create(dataBlockTag.HmiTagname) : connexion.UnifiedDevice.Device.Tags.Create(dataBlockTag.HmiTagname, dataBlockTag.FolderName);
                hmiTag.Connection = connexion.Name;
                hmiTag.PlcTag = dataBlockTag.Tagname;
                
            }
        }
    }


    //public void UnifiedTag(Models.UnifiedTag unifiedTag)
    //{
    //    var plcTag = UnifiedPlcTagNormalized(unifiedTag.PlcTag);

    //    var hmiTags = unifiedTag.Hmi.Device.Tags.Where(c => c.PlcTag == plcTag);
    //    foreach (var hmiTag in hmiTags)
    //    {
    //        var tempAlarms = unifiedTag.Hmi.Device.DiscreteAlarms.Where(w => w.RaisedStateTag == hmiTag.Name).Select(s => s.Name);
    //        foreach (var alarm in tempAlarms)
    //        {
    //            unifiedTag.Hmi.Device.DiscreteAlarms.Single(s => s.Name == alarm)?.Delete();
    //        }

    //        if (hmiTags.Count() > 1)
    //        {
    //            hmiTag.Delete();
    //        }
    //    }

    //    HmiTag tag;
    //    if (!unifiedTag.Hmi.Device.Tags.Any(c => c.PlcTag == plcTag))
    //    {
    //        if (string.IsNullOrEmpty(unifiedTag.Folder))
    //            tag = unifiedTag.Hmi.Device.Tags.Create(unifiedTag.Tagname);
    //        else
    //            tag = unifiedTag.Hmi.Device.Tags.Create(unifiedTag.Tagname, unifiedTag.Folder);
    //    }
    //    else
    //    {
    //        tag = unifiedTag.Hmi.Device.Tags.Single(c => c.PlcTag == plcTag);
    //    }
    //    tag.Connection = unifiedTag.Hmi.Device.Connections.Single(s => s.Partner == unifiedTag.Connexion).Name;
    //    tag.PlcTag = plcTag;
    //    tag.Name = unifiedTag.Tagname;

    //}

    //public void UnifiedTag(Models.Devices.HmiUnifiedDevice device, IEnumerable<Models.UnifiedTag> unifiedTags)
    //{
    //    var index = 0;
    //    var plcTagsUpadted = new List<string>();
    //    var plcTagsAlarmRemove = new List<string>();
    //    do
    //    {
    //        var plcTag = device.Device.Tags[index].PlcTag;

    //        if (plcTagsUpadted.Contains(plcTag))
    //        {
    //            plcTagsAlarmRemove.Add(plcTag);
    //            device.Device.Tags[index].Delete();

    //        }
    //        else if (unifiedTags.Any(a => a.PlcTag == plcTag) && !plcTagsUpadted.Contains(plcTag))
    //        {
    //            plcTagsUpadted.Add(plcTag);

    //            device.Device.Tags[index].Name = UnifiedPlcTagNormalized(unifiedTags.Single(a => a.PlcTag == plcTag).Tagname);
    //            //device.Device.Tags[index].TagTableName = unifiedTags.Single(a => a.PlcTag == plcTag).Folder;
    //            index++;

    //        }
    //        else
    //        {
    //            index++;
    //        }
    //    } while (index < device.Device.Tags.Count);

    //    foreach (var tag in unifiedTags)
    //    {
    //        if (!plcTagsUpadted.Contains(tag.PlcTag))
    //        {
    //            HmiTag hmiTag;
    //            if (string.IsNullOrEmpty(tag.Folder))
    //                hmiTag = device.Device.Tags.Create(UnifiedPlcTagNormalized(tag.Tagname));
    //            else
    //                hmiTag = device.Device.Tags.Create(UnifiedPlcTagNormalized(tag.Tagname), tag.Folder);

    //            hmiTag.Connection = device.Device.Connections.Single(s => s.Partner == tag.Connexion).Name;
    //            hmiTag.PlcTag = tag.PlcTag;
    //        }
    //    }
    //}

    private void BuildDiscretAlarms(IEnumerable<Models.ProjectTree.Connexion> connexions)
    {
        foreach (var connexion in connexions)
        {
            Dictionary<Models.HmiDataBlockTag, IEnumerable<string>> tagnames = [];
            if (connexion.UnifiedDevice is null || connexion.Tags is null) continue;
            var hmiTags = connexion.UnifiedDevice.Device.Tags.ToList();
            var discreteAlarms = connexion.UnifiedDevice.Device.DiscreteAlarms.ToList();

            foreach (var dataBlockTag in connexion.Tags)
            {
                var tagname = UnifiedPlcTagNormalized($"{dataBlockTag.DataBlockName}.{dataBlockTag.FullName}");
                foreach (var hmiTag in hmiTags.Where(unifiedTag => unifiedTag.PlcTag == tagname))
                {
                    //tagnames.Add(dataBlockTag, discreteAlarms.FindAll(f => f.RaisedStateTag == hmiTag.Name).Select(s => s.Name));

                    var alarms = discreteAlarms.Find(f => f.RaisedStateTag == tagname) ?? connexion.UnifiedDevice.Device.DiscreteAlarms.Create(dataBlockTag.HmiTagname);
                    alarms.RaisedStateTag = dataBlockTag.HmiTagname;

                    if(!string.IsNullOrEmpty(dataBlockTag.ClassAlarm))
                        alarms.AlarmClass = dataBlockTag.ClassAlarm;

                    alarms.Origin = dataBlockTag.FolderName;

                    if (opennessService.ProjectLanguages?.Count > 0)
                    {
                        foreach (var lang in opennessService.ProjectLanguages)
                        {
                            if (alarms.EventText.Items.Single(s => s.Language.Culture.Name == lang.Name) is MultilingualTextItem multilingualText)
                            {
                                var txt = string.Empty;
                                dataBlockTag.Descriptions?.TryGetValue(lang, out txt);
                                multilingualText.Text = $"<body><p>{txt}</p></body>";
                            }
                        }
                    }
                }
            }


        }
    }

    //public void UnifiedAlarm(Models.UnifiedAlarm unifiedAlarm)
    //{
    //    var alarms = unifiedAlarm.Hmi.Device.DiscreteAlarms.Find(unifiedAlarm.Tagname) ?? unifiedAlarm.Hmi.Device.DiscreteAlarms.Create(unifiedAlarm.Tagname);
    //    alarms.RaisedStateTag = unifiedAlarm.Tagname;
    //    alarms.AlarmClass = unifiedAlarm.Hmi.Device.AlarmClasses.Single(s => s.Name == unifiedAlarm.ClassName)?.Name;
    //    alarms.Origin = unifiedAlarm.Origin;

    //    if (opennessService.ProjectLanguages?.Count > 0)
    //    {
    //        foreach (var lang in opennessService.ProjectLanguages)
    //        {
    //            if (alarms.EventText.Items.Single(s => s.Language.Culture.Name == lang.Name) is MultilingualTextItem multilingualText)
    //            {
    //                unifiedAlarm.Descriptions.TryGetValue(lang.Name, out string txt);
    //                multilingualText.Text = $"<body><p>{txt}</p></body>";
    //            }
    //        }
    //    }

    //}

    private async Task<List<string>?> ExportTagTable(IEnumerable<Models.ProjectTree.Connexion> connexions)
    {
        List<string>? exportPaths = null;
        foreach (var connexion in connexions)
        {
            if (connexion.UnifiedDevice is null || connexion.Blocks is null) continue;

            //List<string> tagTables = [];
            //foreach (var block in connexion.Blocks)
            //{
            //    var tableTagPath = @$"{block.Path.Remove(connexion.PlcName)}";
            //    tagTables.Add(tableTagPath);

            //    var paths = tableTagPath.Split('\\').Skip(1).ToList();
            //    var tableName = paths.Last();
            //    var hmiTagTablesGroup = connexion.UnifiedDevice.Device.TagTableGroups;
            //    var hmiTagTables = connexion.UnifiedDevice.Device.TagTables;
            //    paths.Remove(tableName);
            //    foreach (var folderName in paths)
            //    {
            //        hmiTagTables = hmiTagTablesGroup.Find(folderName)?.TagTables;
            //        hmiTagTablesGroup = hmiTagTablesGroup.Find(folderName).Groups;
            //    }

            //    if (hmiTagTables?.Find(tableName) is HmiTagTable hmiTagTable)
            //    {
            //        exportPaths ??= [];
            //        exportPaths.AddRange(await opennessService.ExportAsync(hmiTagTable));
            //    }
            //}

            foreach (var item in connexion.UnifiedDevice.Device.TagTables)
            {
                await opennessService.ExportAsync(item);
            }

        }

        return exportPaths;
    }
}
