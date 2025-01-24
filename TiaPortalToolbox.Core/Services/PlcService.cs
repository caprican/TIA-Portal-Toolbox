using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW;

using TiaPortalToolbox.Core.Contracts.Services;
using System.Xml.Serialization;
using System.Globalization;
using SimaticML.SW.Common;
using SimaticML.SW.InterfaceSections;
using System.Text.RegularExpressions;
using TiaPortalToolbox.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Siemens.Engineering;

namespace TiaPortalToolbox.Core.Services;

public class PlcService(IOpennessService opennessService) : IPlcService
{
    private readonly IOpennessService opennessService = opennessService;
    
    public async Task GetMetaDataBlockAsync(Models.ProjectTree.Plc.Object plcItem)
    {
        var fileNames = await opennessService.ExportAsync(plcItem);
        if(fileNames?.Length > 0)
        {
            foreach (var fileName in fileNames)
            {
                try
                {
                    var attrOverrides = new XmlAttributeOverrides();
                    var attrs = new XmlAttributes();

                    var attr = new XmlElementAttribute();
                    attr.Type = typeof(Comment_T_v2);
                    attrs.XmlElements.Add(attr);

                    attrOverrides.Add(typeof(Member_T_v5), attrs);


                    var serializer = new XmlSerializer(typeof(SimaticML.Document), attrOverrides);
                var myFile = new FileStream(fileName, FileMode.Open);
                if (serializer.Deserialize(myFile) is SimaticML.Document document)
                {
                    //foreach (var item in document.Items)
                    //{
                    //    foreach (var multilingualText in item.ObjectList.OfType<SimaticML.MultilingualText_T>().FirstOrDefault().ObjectList.OfType<SimaticML.MultilingualTextItem_T>())
                    //    {
                    //        plcItem.Languages ??= [];
                    //        var culture = CultureInfo.GetCultureInfo(multilingualText.AttributeList.Culture);
                    //        if (!plcItem.Languages.Contains(culture))
                    //        {
                    //            plcItem.Languages.Add(culture);
                    //        }
                    //    }

                    //    //((Models.ProjectTree.Plc.Blocks.Object)plcItem).Members = item switch
                    //    //{
                    //    //    SimaticML.SW.Blocks.FC fc => GetInterfaceMembers(fc.AttributeList.Interface.Sections, plcItem.Languages),
                    //    //    SimaticML.SW.Blocks.FB fb => GetInterfaceMembers(fb.AttributeList.Interface.Sections, plcItem.Languages),
                    //    //    _ => null,
                    //    //};

                    //    //var compileUnits = item switch
                    //    //{
                    //    //    SimaticML.SW.Blocks.FC fc => GetCompileUnits(fc.ObjectList.OfType<SimaticML.SW.Blocks.CompileUnit>(), plcItem.Languages),
                    //    //    SimaticML.SW.Blocks.FB fb => GetCompileUnits(fb.ObjectList.OfType<SimaticML.SW.Blocks.CompileUnit>(), plcItem.Languages),
                    //    //    _ => null,
                    //    //};


                    //    //if (compileUnits?.Count > 0)
                    //    //{
                    //    //    foreach (var compileUnit in compileUnits)
                    //    //    {
                    //    //        if (compileUnit[PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("BLOCK INFO HEADER", StringComparison.InvariantCultureIgnoreCase)))
                    //    //        {
                    //    //            GetNetworkHeader(compileUnit[PlcNetworkCommentType.Comment], plcItem);
                    //    //        }
                    //    //        else if (compileUnit[Core.Models.PlcNetworkCommentType.Title].Any(a => a.Value.Trim().Equals("DESCRIPTION", StringComparison.InvariantCultureIgnoreCase)))
                    //    //        {
                    //    //            ((Models.ProjectTree.Plc.Blocks.Object)plcItem).Description = compileUnit[PlcNetworkCommentType.Comment];
                    //    //        }
                    //    //    }
                    //    //}


                    //}
                }

                }
                catch 
                { 
                }
            }
        }

    }


    internal Dictionary<CultureInfo, List<InterfaceMember>>? GetInterfaceMembers(Section_T[] sections, IEnumerable<CultureInfo> languages)
    {
        Dictionary<CultureInfo, List<InterfaceMember>>? cultureMembers = null;
        foreach (var language in languages)
        {
            List<InterfaceMember>? members = null;
            //foreach (var section in sections.Where(s => s.Name == SectionName_TE.Input ||
            //                                            s.Name == SectionName_TE.Output ||
            //                                            s.Name == SectionName_TE.Input))
            //{
            //    if (section.Member is not null)
            //    {
            //        foreach (var member in section.Member)
            //        {
            //            members ??= [];
            //            members.Add(new InterfaceMember
            //            {
            //                Name = member.Name,
            //                Direction = section.Name switch
            //                {
            //                    SectionName_TE.Input => DirectionMember.Input,
            //                    SectionName_TE.InOut => DirectionMember.InOutput,
            //                    SectionName_TE.Output => DirectionMember.Output,
            //                    SectionName_TE.Return => DirectionMember.Return,
            //                    SectionName_TE.Static => DirectionMember.Static,
            //                    _ => DirectionMember.Other
            //                },
            //                Type = GetMemberType(member.Datatype),
            //                //DerivedType = GetDerivedType(member.de),
            //                Description = GetMemberComment(member.Items.OfType<Comment_T>(), language),
            //                Islocked = section.Name == SectionName_TE.Return,
            //                DefaultValue = GetMemberStartValue(member.Items.OfType<StartValue_T>()),
            //                HidenInterface = member.AttributeList?.Any(attr => attr is StringAttribute_T hiddenAssign && hiddenAssign.Name == "S7_HiddenAssignment" && hiddenAssign.Value == "Hide") == true
            //            });

            //        }
            //    }
            //}

            if(members?.Count > 0)
            {
                cultureMembers ??= [];
                cultureMembers.Add(language, members);
            }
        }

        return cultureMembers;
    }

    private string GetMemberType(string memberType)
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

    //private string GetDerivedType(string memberType)
    //{
    //    if (Regex.Match(memberType, @"(.*?) of ""(.*?)""") is Match arrayMatch && arrayMatch.Success)
    //    {
    //        return arrayMatch.Groups[2].Value;
    //    }
    //    else if (Regex.Match(memberType, @"""(.*?)""") is Match udtMatch && udtMatch.Success)
    //    {
    //        return udtMatch.Groups[1].Value;
    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}

    private Dictionary<string, string> GetMemberComment(IEnumerable<Comment_T> comments)
    {
        var description = new Dictionary<string, string>();
        //if (comments is not null)
        //{
        //    foreach (var comment in comments)
        //    {
        //        if (comment.MultiLanguageText is not null)
        //        {
        //            foreach (var text in comment.MultiLanguageText)
        //            {
        //                description.Add(text.Lang, text.Value);
        //            }
        //        }
        //    }
        //}
        return description;
    }

    private string? GetMemberComment(IEnumerable<Comment_T> comments, CultureInfo language)
    {
        //if (comments?.Count() > 0)
        //{
        //    foreach (var comment in comments)
        //    {
        //        return comment.MultiLanguageText?.SingleOrDefault(s => s.Lang == language.Name)?.Value;
        //    }
        //}
        return null;
    }

    private string GetMemberStartValue(IEnumerable<StartValue_T> startValues)
    {
        foreach (var value in startValues)
        {
            return value.Value;
        }

        return string.Empty;
    }

    private List<Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>> GetCompileUnits(IEnumerable<SimaticML.SW.Blocks.CompileUnit> compileUnits, List<CultureInfo>? languages)
    {
        List<Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>> results = [];
        //foreach (var compileUnit in compileUnits)
        //{
        //    switch (compileUnit.AttributeList.ProgrammingLanguage)
        //    {
        //        case SimaticML.ProgrammingLanguage_TE.SCL:
        //            foreach (var region in StructuredTextRegions(compileUnit.AttributeList.NetworkSource))
        //            {
        //                results.Add(NetworkComment(region, languages));
        //            }

        //            break;
        //        case SimaticML.ProgrammingLanguage_TE.STL:
        //        case SimaticML.ProgrammingLanguage_TE.LAD:
        //        case SimaticML.ProgrammingLanguage_TE.FBD:
        //        case SimaticML.ProgrammingLanguage_TE.F_FBD:
        //        case SimaticML.ProgrammingLanguage_TE.F_LAD:
        //            results.Add(NetworkComment(compileUnit.ObjectList.OfType<SimaticML.MultilingualText_T>()));
        //            break;
        //    }
        //}

        return results;
    }

    private List<List<object>> StructuredTextRegions(SimaticML.NetworkSource_T networkSource)
    {
        List<List<object>> regions = [];
        var structuredTexts = networkSource.StructuredText?.ToList();
        if (structuredTexts is not null)
        {
            foreach (var structuredText in structuredTexts)
            {
                var SclItems = structuredText.Items.ToList();
                var regionTags = SclItems.FindAll(item => item is Token_T token && (token.Text == "REGION" || token.Text == "END_REGION"));
                if(regionTags?.Count > 0)
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
                else if(regionTags is not null)
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

    private Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>> NetworkComment(IEnumerable<SimaticML.MultilingualText_T> multilingualTexts)
    {
        var dictionaryTexts = new Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>();
        foreach (var multilingualText in multilingualTexts)
        {
            var texts = new Dictionary<CultureInfo, string>();
            foreach (var multilingualTextItem in multilingualText.ObjectList.OfType<SimaticML.MultilingualTextItem_T>().Where(item => item.CompositionName == "Items"))
            {
                texts.Add(CultureInfo.GetCultureInfo(multilingualTextItem.AttributeList.Culture), multilingualTextItem.AttributeList.Text);
            }
            switch (multilingualText.CompositionName)
            {
                case "Title":
                    dictionaryTexts.Add(PlcNetworkCommentType.Title, texts);
                    break;
                case "Comment":
                    dictionaryTexts.Add(PlcNetworkCommentType.Comment, texts);
                    break;
            }
        }

        return dictionaryTexts;
    }

    private Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>> NetworkComment(List<object> region, List<CultureInfo>? languages)
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

        var dictionaryTexts = new Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>
        {
            { PlcNetworkCommentType.Title, texts }
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
                    case LineComment_T lineComment:
                        languages?.ForEach(culture => texts[culture] += string.Join(" ", lineComment.Items.OfType<Text_T>().Select(s => s.Value)));
                        break;
                    case NewLine_T:
                        languages?.ForEach(culture => texts[culture] += "\r\n");
                        break;
                    case Comment_T comment:
                        //foreach (var multiLanguageText in comment.MultiLanguageText)
                        //{
                        //    texts[CultureInfo.GetCultureInfo(multiLanguageText.Lang)] += multiLanguageText.Value;
                        //}
                        break;
                }
            }
        }

        dictionaryTexts.Add(PlcNetworkCommentType.Comment, texts);

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
