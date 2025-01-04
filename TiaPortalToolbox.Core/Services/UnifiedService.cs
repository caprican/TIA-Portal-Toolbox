using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using Siemens.Engineering;
using Siemens.Engineering.Hmi.Tag;
using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HmiUnified.HmiTags;
using Siemens.Engineering.HW;

using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Core.Models;
using TiaPortalToolbox.Core.Models.ProjectTree;
using TiaPortalToolbox.Core.Models.ProjectTree.Devices;

namespace TiaPortalToolbox.Core.Services;

public class UnifiedService(IOpennessService opennessService) : IUnifiedService
{
    private readonly IOpennessService opennessService = opennessService;

    private string UnifiedPlcTagNormalized(string tagname)
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

    public string GetAlarmsClassName(Unified hmiUnified, string classname, string tagname, string defaultClassname)
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

    public Task BuildHmiTags(IEnumerable<Connexion> connexions, string defaultAlarmsClass, bool simplifyTagname)
    {
        var tcs = new TaskCompletionSource<IEnumerable<Connexion>?>();
        ThreadPool.QueueUserWorkItem(_ =>
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
                    ExtractTagAlarms(connexions, defaultAlarmsClass, simplifyTagname);


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

    private List<InterfaceMember> GetInterfaceMember(SimaticML.SW.InterfaceSections.Member_T member, string parent = "")
    {
        var tags = new List<InterfaceMember>();
        var subTags = new List<InterfaceMember>();
        var tag = new InterfaceMember
        {
            Name = string.IsNullOrEmpty(parent) ? member.Name : $"{parent}.{member.Name}",
            Direction = DirectionMember.Static,
            Type = member.Datatype
        };

        foreach (var item in member.Items)
        {
            switch (item)
            {
                case SimaticML.SW.Common.Comment comment:
                    foreach (var multiLangugageText in comment.MultiLanguageText)
                    {
                        tag.Description ??= [];
                        tag.Description.Add(multiLangugageText.Lang, multiLangugageText.Value);
                    }
                    break;
                case SimaticML.SW.InterfaceSections.Member_T memberItem:
                    subTags.AddRange(GetInterfaceMember(memberItem, tag.Name));
                    break;
            }
        }
        tags.Add(tag);
        tags.AddRange(subTags);

        return tags;
    }

    private void ExtractTagAlarms(IEnumerable<Connexion> connexions, string defaultAlarmsClass, bool simplifyTagname)
    {
        foreach (var connexion in connexions)
        {
            if (connexion.Blocks?.Count() > 0 && connexion.PlcDevice is not null && connexion.UnifiedDevice is not null)
            {
                var log = $"{connexion.UnifiedDevice.Name} <=> {connexion.PlcDevice.Name}";
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
                                foreach (var exportedGlobalDataBlock in document.Items.Where(sw => sw is SimaticML.SW.Blocks.GlobalDB).Cast<SimaticML.SW.Blocks.GlobalDB>())
                                {
                                    var exportGlobalDB = exportedGlobalDataBlock.AttributeList.Interface.Sections.FirstOrDefault(section => section.Name == SimaticML.SW.Common.SectionName_TE.Static);
                                    if (exportGlobalDB is not null)
                                    {

                                        Debug.WriteLine($"{DateTime.Now.ToLocalTime()} - {log} : {block.Name}");

                                        var alarmsClassName = defaultAlarmsClass;
                                        foreach (var exportedMember in exportGlobalDB.Member)
                                        {
                                            foreach (var member in GetInterfaceMember(exportedMember))
                                            {
                                                switch (member.Type)
                                                {
                                                    case "Struct":

                                                        foreach (var lang in opennessService.ProjectLanguages)
                                                        {
                                                            if (member.Description?.ContainsKey(lang.Name) == true)
                                                            {
                                                                var txt = member.Description[lang.Name];
                                                                if (!string.IsNullOrEmpty(txt))
                                                                {
                                                                    var match = Regex.Match(txt, @"\[AlarmClass=(.+?)\]", RegexOptions.IgnoreCase);
                                                                    alarmsClassName = GetAlarmsClassName(connexion.UnifiedDevice, match.Groups[1].Value, member.Name, defaultAlarmsClass);
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        //Debug.WriteLine($"{DateTime.Now.ToLocalTime()} - {log} : {globalDb.Block.Name} class found {member.Name}");
                                                        break;
                                                    case "Bool":
                                                        //tags.Add(new TIAOpennessAdapter.Models.UnifiedTag
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

    //public void UnifiedTagsFolder(Models.UnifiedTagsFolder unifiedTagsFolder)
    //{
    //    var tagTables = new List<HmiTagTable>();
    //    tagTables.AddRange(unifiedTagsFolder.Hmi.Device.TagTables);

    //    var tagTablesGroup = new List<HmiTagTableGroup>();
    //    tagTablesGroup.AddRange(unifiedTagsFolder.Hmi.Device.TagTableGroups);
    //    do
    //    {
    //        if (tagTablesGroup.Count > 0)
    //        {
    //            tagTables.AddRange(tagTablesGroup[0].TagTables);
    //            tagTablesGroup.AddRange(tagTablesGroup[0].Groups);
    //            tagTablesGroup.RemoveAt(0);
    //        }
    //    } while (tagTablesGroup.Count > 0);

    //    if (!tagTables.Any(a => a.Name == unifiedTagsFolder.Name))
    //        unifiedTagsFolder.Hmi.Device.TagTables.Create(unifiedTagsFolder.Name);
    //}

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
}
