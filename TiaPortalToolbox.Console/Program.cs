// See https://aka.ms/new-console-template for more information
using SimaticML.SW.Common;
using SimaticML.SW.InterfaceSections;

using System.Xml;
using System.Xml.Serialization;

Console.WriteLine("Hello, World!");

var fileName = @"C:\Users\capri\OneDrive\Documents\Automation\TIA069\UserFiles\Export\N100_Defauts.xml";

//var attrOverrides = new XmlAttributeOverrides();
//var attrs = new XmlAttributes();

//var attr = new XmlElementAttribute
//{
//    ElementName = "Comment",
//    Type = typeof(Comment_T_v2)
//};
//attrs.XmlElements.Add(attr);

//attrOverrides.Add(typeof(Member_T_v5), attrs);

var settings = new XmlReaderSettings
{
    //ValidationType = ValidationType.Schema
};
//settings.Schemas.Add("", @"Schemas\Commons\SW.Common_v3.xsd");
//settings.Schemas.Add("http://www.siemens.com/automation/Openness/SW/Interface/v5", @"Schemas\InterfaceSections\SW.InterfaceSections_v5.xsd");


var serializer = new XmlSerializer(typeof(SimaticML.Document)/*, attrOverrides*/);
serializer.UnknownAttribute += OnUnknownAttribute;
serializer.UnknownElement += OnUnknownElement;

//var myFile = new FileStream(fileName, FileMode.Open);
var reader = XmlReader.Create(fileName, settings);

//if (serializer.Deserialize(myFile) is SimaticML.Document document)
if (serializer.Deserialize(reader) is SimaticML.Document document)
{
    switch(document.Items[0])
    {
        case SimaticML.SW.Blocks.GlobalDB globalDB:
            var item = globalDB.AttributeList.Interface.Sections[0];//.Member[0];

            break;
    }

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


void OnUnknownElement(object? sender, XmlElementEventArgs e)
{
}

void OnUnknownAttribute(object? sender, XmlAttributeEventArgs e)
{
}