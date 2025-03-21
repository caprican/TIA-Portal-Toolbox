﻿using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using Siemens.Engineering.SW.Blocks;

using SimaticML.SW.Common;
using SimaticML.SW.InterfaceSections;

using TiaPortalToolbox.Core.Contracts.Services;

namespace TiaPortalToolbox.Core.Services;

public class PlcService(IOpennessService opennessService) : IPlcService
{
    private readonly IOpennessService opennessService = opennessService;
    
    public async Task GetMetaDataBlockAsync(Models.ProjectTree.Plc.Object? plcItem)
    {
        var fileNames = await opennessService.ExportAsync(plcItem);
        if(fileNames?.Length > 0)
        {
            foreach (var fileName in fileNames)
            {
                switch(plcItem)
                {
                    case Models.ProjectTree.Plc.Type plcType:
                        ReadPlcBlock(fileName, plcType);
                        break;
                    case Models.ProjectTree.Plc.Blocks.Object plcBlock :
                        ReadPlcBlock(fileName, plcBlock);
                        break;
                    default:
                        GetBlock(fileName, plcItem);
                        break;
                }
            }
        }
    }

    public async Task GetMetaDataBlockAsync(string fileName)
    {
        ReadPlcBlock(fileName, null);
    }

    private void GetBlock(string fileName, Models.ProjectTree.Object? plcItem = null)
    {
        var plcChild = plcItem?.Items?.SingleOrDefault(f => f.Name == Path.GetFileNameWithoutExtension(fileName));
        if (plcChild is Models.ProjectTree.Plc.Object item)
        {
            ReadPlcBlock(fileName, item);
        }
        else
        {
            if(plcItem?.Items is not null)
            {
                foreach (var subitem in plcItem.Items)
                {
                    GetBlock(fileName, subitem);
                }
            }
        }
        
    }

    private Models.ProjectTree.Plc.Object ReadPlcBlock(string fileName, Models.ProjectTree.Plc.Object? plcItem)
    {
        var serializer = new XmlSerializer(typeof(SimaticML.Document));
        var myFile = new FileStream(fileName, FileMode.Open);
        if (serializer.Deserialize(myFile) is SimaticML.Document document)
        {
            Debug.WriteLine($"Deserialize file : {fileName}");
            foreach (var item in document)
            {
                plcItem ??= item switch
                {
                    SimaticML.SW.Blocks.FC fc => new Models.ProjectTree.Plc.Blocks.Fc(null, ""),
                    SimaticML.SW.Blocks.FB fb => new Models.ProjectTree.Plc.Blocks.Fb(null, ""),
                    SimaticML.SW.Blocks.GlobalDB globalDb => new Models.ProjectTree.Plc.Blocks.DataBlocks.GlobalData(null, ""),
                    SimaticML.SW.Blocks.ArrayDB arrayDb => new Models.ProjectTree.Plc.Blocks.DataBlocks.ArrayDb(null, ""),
                    SimaticML.SW.Types.PlcStruct plcStruct => new Models.ProjectTree.Plc.Type(null, ""),
                    _ => new Models.ProjectTree.Plc.Item(null, ""),
                };

                foreach (var multilingualText in item.OfType<SimaticML.MultilingualText_T>().FirstOrDefault().OfType<SimaticML.MultilingualTextItem_T>())
                {
                    plcItem.Languages ??= [];
                    var culture = CultureInfo.GetCultureInfo(multilingualText.Culture);
                    if (!plcItem.Languages.Contains(culture))
                    {
                        plcItem.Languages.Add(culture);
                    }
                }

                switch(plcItem)
                {
                    case Models.ProjectTree.Plc.Blocks.Object block:
                        block.Members = item switch
                        {
                            SimaticML.SW.Blocks.FC fc => GetInterfaceMembers(fc.Attributes.Interface, plcItem.Languages),
                            SimaticML.SW.Blocks.FB fb => ((Func<Dictionary<CultureInfo, List<Models.InterfaceMember>>>) (() =>
                            {
                                Dictionary<CultureInfo, List<Models.InterfaceMember>>? members = [];
                                var interfaceMembers = GetInterfaceMembers(fb.Attributes.Interface, plcItem.Languages);
                                if (interfaceMembers is not null)
                                {
                                    members.AddRange(interfaceMembers);
                                }

                                var staticMembers = GetMembers(fb.Attributes.Interface, plcItem.Languages);
                                if(staticMembers is not null)
                                {
                                    foreach (var item in staticMembers)
                                    {
                                        members[item.Key].AddRange(item.Value);
                                    }
                                }

                                return members;
                            }))(),
                            SimaticML.SW.Blocks.GlobalDB globalDb => GetMembers(globalDb.Attributes.Interface, plcItem.Languages),
                            SimaticML.SW.Blocks.ArrayDB arrayDb => GetMembers(arrayDb.Attributes.Interface, plcItem.Languages),
                            _ => null,
                        };
                        break;
                    case Models.ProjectTree.Plc.Type type:
                        type.Members = item switch
                        {
                            SimaticML.SW.Types.PlcStruct plcStruct => GetMembers(plcStruct.Attributes.Interface, plcItem.Languages),
                            _ => null,
                        };
                        break;
                }


                var programmingLanguage = item switch
                {
                    SimaticML.SW.Blocks.FC fc => fc.Attributes.ProgrammingLanguage,
                    SimaticML.SW.Blocks.FB fb => fb.Attributes.ProgrammingLanguage,

                    SimaticML.SW.Blocks.GlobalDB globalDb => globalDb.Attributes.ProgrammingLanguage,
                    SimaticML.SW.Blocks.ArrayDB arrayDb => arrayDb.Attributes.ProgrammingLanguage,
                    _ => null,
                };

                var compileUnits = item switch
                {
                    SimaticML.SW.Blocks.FC fc => GetCompileUnits(fc.OfType<SimaticML.SW.Blocks.CompileUnit>(), programmingLanguage, plcItem.Languages),
                    SimaticML.SW.Blocks.FB fb => GetCompileUnits(fb.OfType<SimaticML.SW.Blocks.CompileUnit>(), programmingLanguage, plcItem.Languages),
                    _ => null,
                };

                if (compileUnits?.Count > 0)
                {
                    foreach (var compileUnit in compileUnits)
                    {
                        if (compileUnit[Core.Models.PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("BLOCK INFO HEADER", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            GetNetworkHeader(compileUnit[Core.Models.PlcNetworkCommentType.Comment], plcItem);
                        }
                        else if (compileUnit[Core.Models.PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("PREAMBLE", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ((Models.ProjectTree.Plc.Blocks.Object)plcItem).Preamble = compileUnit[Core.Models.PlcNetworkCommentType.Comment];
                        }
                        else if (compileUnit[Core.Models.PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("DESCRIPTION", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ((Models.ProjectTree.Plc.Blocks.Object)plcItem).Descriptions = compileUnit[Core.Models.PlcNetworkCommentType.Comment];
                        }
                        else if (compileUnit[Core.Models.PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("APPENDIX", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ((Models.ProjectTree.Plc.Blocks.Object)plcItem).Appendix = compileUnit[Core.Models.PlcNetworkCommentType.Comment];
                        }
                    }
                }
            }
        }

        return plcItem;
    }

    public async Task<Models.ProjectTree.Plc.Object?> GetItem(string name) => await opennessService.GetItem(name) as Models.ProjectTree.Plc.Object;


    internal Dictionary<CultureInfo, List<Core.Models.InterfaceMember>>? GetInterfaceMembers(SimaticML.SW.Interface_T @interface, IEnumerable<CultureInfo>? languages)
    {
        Dictionary<CultureInfo, List<Core.Models.InterfaceMember>>? cultureMembers = null;

        if(languages is null) return null;

        foreach (var language in languages)
        {
            List<Core.Models.InterfaceMember>? members = null;
            foreach (var sections in @interface)
            {
                foreach (var section in sections.Where(s => s.Name == SectionName_TE.Input ||
                                                            s.Name == SectionName_TE.Output ||
                                                            s.Name == SectionName_TE.InOut))
                {
                    if (section.Members is not null)
                    {
                        foreach (var member in section.Members)
                        {
                            members ??= [];
                            members.Add(GetMember(section.Name, member));

                        }
                    }
                }
            }

            if (members?.Count > 0)
            {
                cultureMembers ??= [];
                cultureMembers.Add(language, members);
            }
        }

        return cultureMembers;
    }

    internal Dictionary<CultureInfo, List<Core.Models.InterfaceMember>>? GetMembers(SimaticML.SW.Interface_T @interface, IEnumerable<CultureInfo>? languages)
    {
        Dictionary<CultureInfo, List<Core.Models.InterfaceMember>>? cultureMembers = null;

        if (languages is null) return null;

        foreach (var language in languages)
        {
            List<Core.Models.InterfaceMember>? members = null;
            foreach (var sections in @interface)
            {
                foreach (var section in sections.Where(s => s.Name == SectionName_TE.Static || s.Name == SectionName_TE.None))
                {
                    if (section.Members is not null)
                    {
                        foreach (var member in section.Members)
                        {
                            members ??= [];
                            members.Add(GetMember(section.Name, member));
                        }
                    }
                }
            }

            if (members?.Count > 0)
            {
                cultureMembers ??= [];
                cultureMembers.Add(language, members);
            }
        }

        return cultureMembers;
    }

    private Core.Models.InterfaceMember GetMember(SectionName_TE sectionName, IMember_T member)
    {
        var result = new Core.Models.InterfaceMember
        {
            Name = member.Name,
            Direction = sectionName switch
            {
                SectionName_TE.Input => Core.Models.DirectionMember.Input,
                SectionName_TE.InOut => Core.Models.DirectionMember.InOutput,
                SectionName_TE.Output => Core.Models.DirectionMember.Output,
                SectionName_TE.Return => Core.Models.DirectionMember.Return,
                SectionName_TE.Static => Core.Models.DirectionMember.Static,
                _ => Core.Models.DirectionMember.Other
            },
            Type = GetMemberType(member.Datatype),
            DerivedType = GetDerivedType(member.Datatype),
            Descriptions = GetMemberComment(member.Comment),
            Islocked = sectionName == SectionName_TE.Return,
            DefaultValue = GetMemberStartValue(member.OfType<StartValue_T>()),
            HidenInterface = member.Attributes?.Any(attr => attr is StringAttribute_T hiddenAssign && hiddenAssign.Name == "S7_HiddenAssignment" && hiddenAssign.Value == "Hide") == true
        };

        if (member.Count() > 0)
        {
            foreach(IMember_T child in member.OfType<Member_T>())
            {
                result.Members ??= [];
                result.Members.Add(GetMember(sectionName, child));
            }
        }

        return result;
    }


    private string? GetMemberType(string memberType)
    {
        if (Regex.Match(memberType, @"(.*?) of ""(.*?)""") is Match arrayMatch && arrayMatch.Success)
        {
            return $"{arrayMatch.Groups[1].Value} of {arrayMatch.Groups[2].Value}";
        }
        else if (Regex.Match(memberType, @"""(.*?)""") is Match udtMatch && udtMatch.Success)
        {
            return udtMatch.Groups[1].Value;
        }
        else
        {
            return memberType;
        }
    }

    private string? GetDerivedType(string memberType)
    {
        if (Regex.Match(memberType, @"(.*?) of ""(.*?)""") is Match arrayMatch && arrayMatch.Success)
        {
            return arrayMatch.Groups[2].Value;
        }
        else if (Regex.Match(memberType, @"""(.*?)""") is Match udtMatch && udtMatch.Success)
        {
            return udtMatch.Groups[1].Value;
        }
        else
        {
            return null;
        }
    }

    private string? GetMemberComment(IComment_T comments, CultureInfo language) =>
        comments?.SingleOrDefault(s => s.Lang == language.Name)?.Value;

    private Dictionary<CultureInfo, string>? GetMemberComment(IComment_T? comments)
    {
        var result = new Dictionary<CultureInfo, string>();

        if(comments is not null)
        {
            foreach (var comment in comments)
            {
                result.Add(CultureInfo.GetCultureInfo(comment.Lang), comment.Value);
            }
        }

        return result.Count > 0 ? result : null;
    }

    private string GetMemberStartValue(IEnumerable<StartValue_T> startValues)
    {
        foreach (var value in startValues)
        {
            return value.Value;
        }

        return string.Empty;
    }

    private List<Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>>> GetCompileUnits(IEnumerable<SimaticML.SW.Blocks.CompileUnit> compileUnits, SimaticML.ProgrammingLanguage_TE? programmingLanguage, List<CultureInfo>? languages)
    {
        List<Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>>> results = [];
        foreach (var compileUnit in compileUnits)
        {
            switch (compileUnit.Attributes.ProgrammingLanguage)
            {
                case SimaticML.ProgrammingLanguage_TE.SCL:

                    var comments = NetworkComment(compileUnit.OfType<SimaticML.MultilingualText_T>());

                    var regions = StructuredTextRegions(compileUnit.Attributes.NetworkSource);
                    if(regions?.Count == 0)
                    {
                        results.Add(comments);
                    }
                    else
                    {
                        foreach (var region in regions!)
                        {
                            var networks = NetworkComment(region, languages);

                            if (comments?.TryGetValue(Core.Models.PlcNetworkCommentType.Title, out var title) == true && networks.ContainsKey(Core.Models.PlcNetworkCommentType.Title))
                            {
                                if (networks[Core.Models.PlcNetworkCommentType.Title].All(a => string.IsNullOrEmpty(a.Value)) == true)
                                {
                                    networks[Core.Models.PlcNetworkCommentType.Title] = title;
                                }

                            }
                            results.Add(networks);
                        }
                    }

                    break;
                case SimaticML.ProgrammingLanguage_TE.STL:
                case SimaticML.ProgrammingLanguage_TE.LAD:
                case SimaticML.ProgrammingLanguage_TE.FBD:
                case SimaticML.ProgrammingLanguage_TE.F_FBD:
                case SimaticML.ProgrammingLanguage_TE.F_LAD:
                    results.Add(NetworkComment(compileUnit.OfType<SimaticML.MultilingualText_T>()));
                    break;
            }
        }

        return results;
    }

    private List<List<SimaticML.SW.Object_G>> StructuredTextRegions(SimaticML.NetworkSource_T networkSource)
    {
        List<List<SimaticML.SW.Object_G>> regions = [];
        if (networkSource is not null)
        {
            foreach (var structuredText in networkSource)
            {
                var SclItems = structuredText.ToList();
                var regionTags = SclItems.FindAll(item => item is Token_T token && (token.Text == "REGION" || token.Text == "END_REGION"));
                if (regionTags?.Count > 0)
                {
                    for (var i = 0; i < regionTags.Count; i += 2)
                    {
                        var indexStart = SclItems.IndexOf(regionTags[i]);
                        var indexEnd = SclItems.IndexOf(regionTags[i + 1]);

                        var region = SclItems.Skip(indexStart).Take(indexEnd - indexStart).ToList();
                        if (region.Any(item => item is Text_T || item is LineComment_T || item is Comment_T))
                        {
                            region.RemoveAll(r => r is Blank_T);
                            regions.Add(region);
                        }
                    }
                }
                else if (regionTags is not null)
                {
                    if (SclItems.Any(item => item is Text_T || item is LineComment_T || item is Comment_T))
                    {
                        SclItems.RemoveAll(r => r is Blank_T);
                        regions.Add(SclItems);
                    }
                }
            }
        }
        return regions;
    }

    private Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>> NetworkComment(IEnumerable<SimaticML.MultilingualText_T> multilingualTexts)
    {
        var dictionaryTexts = new Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>>();
        foreach (var multilingualText in multilingualTexts)
        {
            var texts = new Dictionary<CultureInfo, string>();
            foreach (var multilingualTextItem in multilingualText.OfType<SimaticML.MultilingualTextItem_T>().Where(item => item.CompositionName == "Items"))
            {
                texts.Add(CultureInfo.GetCultureInfo(multilingualTextItem.Culture), multilingualTextItem.Text);
            }
            switch (multilingualText.CompositionName)
            {
                case "Title":
                    dictionaryTexts.Add(Core.Models.PlcNetworkCommentType.Title, texts);
                    break;
                case "Comment":
                    dictionaryTexts.Add(Core.Models.PlcNetworkCommentType.Comment, texts);
                    break;
            }
        }

        return dictionaryTexts;
    }

    private Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>> NetworkComment(List<SimaticML.SW.Object_G> region, List<CultureInfo>? languages)
    {
        var indexTitle = region.FindIndex(r => r is NewLine_T);
        var titleContent = region.Take(indexTitle);

        var texts = new Dictionary<CultureInfo, string>();
        var titleText = string.Join(" ", titleContent.OfType<Text_T>().Select(s => s.Value));
        if (languages?.Count > 0)
        {
            foreach (var culture in languages)
            {
                texts.Add(culture, titleText);
            }
        }

        var dictionaryTexts = new Dictionary<Core.Models.PlcNetworkCommentType, Dictionary<CultureInfo, string>>
        {
            { Core.Models.PlcNetworkCommentType.Title, texts }
        };

        texts = [];
        if (languages?.Count > 0)
        {
            foreach (var culture in languages)
            {
                texts.Add(culture, string.Empty);
            }
        }
        if (indexTitle + 1 < region.Count - indexTitle)
        {
            foreach (var itemRegion in region.Skip(indexTitle + 1))
            {
                switch (itemRegion)
                {
                    case ILineComment_T lineComment:
                        languages?.ForEach(culture => texts[culture] += string.Join(" ", lineComment.OfType<Text_T>().Select(s => s.Value)));
                        break;
                    case NewLine_T:
                        languages?.ForEach(culture => texts[culture] += "\r\n");
                        break;
                    case IComment_T comment:
                        foreach (var multiLanguageText in comment)
                        {
                            texts[CultureInfo.GetCultureInfo(multiLanguageText.Lang)] += multiLanguageText.Value;
                        }
                        break;
                }
            }
        }

        dictionaryTexts.Add(Core.Models.PlcNetworkCommentType.Comment, texts);

        return dictionaryTexts;
    }

    private void GetNetworkHeader(Dictionary<CultureInfo, string> blockHeader, Models.ProjectTree.Plc.Object plcItem)
    {
        foreach (var culture in blockHeader)
        {
            foreach (var attribut in Regex.Matches(blockHeader[culture.Key], @"(Title|Author|Comment|Function|Library|Family):(?<item>[^\r\n]*)\r?$", RegexOptions.Multiline).OfType<Match>())
            {
                switch (attribut.Groups[1].Value.Trim())
                {
                    case "Title":
                        plcItem.Title ??= [];
                        plcItem.Title.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                    case "Author":
                        plcItem.Author ??= [];
                        plcItem.Author.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                    case "Comment":
                        plcItem.Comment ??= [];
                        plcItem.Comment.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                    case "Function":
                        plcItem.Function ??= [];
                        plcItem.Function.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                    case "Library":
                        plcItem.Library ??= [];
                        plcItem.Library.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                    case "Family":
                        plcItem.Family ??= [];
                        plcItem.Family.Add(culture.Key, attribut.Groups[2].Value.Trim());
                        break;
                }
            }

            if (Regex.Match(blockHeader[culture.Key], @"Change log table:[\r\n]+") is Match regexLog && regexLog.Success)
            {
                var rows = blockHeader[culture.Key].Substring(regexLog.Index + regexLog.Length).Split('\n');
                var headers = new List<string>();
                foreach (var item in rows[0].Split('|'))
                {
                    headers.Add(item.Trim());
                }

                List<Models.PlcBlockLog>? logs = null;
                foreach (var row in rows.Skip(1))
                {
                    var items = row.Split('|');
                    if (!items.All(columns => columns.Contains('-')) && items.Length >= 3)
                    {
                        var log = new Models.PlcBlockLog
                        {
                            Author = items[headers.IndexOf("Expert in charge")],
                            Version = items[headers.IndexOf("Version")],
                            Description = items[headers.IndexOf("Changes applied")]
                        };

                        if (DateTime.TryParse(items[headers.IndexOf("Date")], out DateTime edited))
                        {
                            log.Edited = edited;
                        }

                        logs ??= [];
                        logs.Add(log);
                    }
                }
                if (logs?.Count > 0)
                {
                    plcItem.Logs ??= [];
                    plcItem.Logs.Add(culture.Key, logs);
                }
            }

        }
    }

}
