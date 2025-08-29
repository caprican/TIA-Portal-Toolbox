using System.Globalization;

using DocSharp.Markdown;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Markdig;

using TiaPortalToolbox.Core.Models;

namespace TiaPortalToolbox.Doc.Helpers;

internal static class DocumentHelper
{
    //internal static Dictionary<OpenXmlStyles, (string Name, int? Level)> Styles = [];

    //public static Paragraph AddStyledParagraphOfText(this MainDocumentPart mainDocumentPart, string styleId, string text)
    //{
    //    var paragraph = CreateParagraphOfText(text);

    //    mainDocumentPart.Document.Body ??= new Body();
    //    mainDocumentPart.Document.Body!.AppendChild(paragraph);

    //    mainDocumentPart.Document.ApplyStyleToParagraph(styleId, paragraph);

    //    return paragraph;
    //}

    //// Create a new style with the specified styleid and stylename and add it to the specified
    //// style definitions part.
    //private static void AddNewStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename)
    //{
    //    // Get access to the root element of the styles part.
    //    Styles? styles = styleDefinitionsPart.Styles;

    //    // Create a new paragraph style and specify some of the properties.
    //    Style style = new()
    //    {
    //        Type = StyleValues.Paragraph,
    //        StyleId = styleid,
    //        CustomStyle = true
    //    };
    //    StyleName styleName1 = new() { Val = stylename };
    //    BasedOn basedOn1 = new() { Val = "Normal" };
    //    NextParagraphStyle nextParagraphStyle1 = new() { Val = "Normal" };
    //    style.Append(styleName1);
    //    style.Append(basedOn1);
    //    style.Append(nextParagraphStyle1);

    //    // Create the StyleRunProperties object and specify some of the run properties.
    //    StyleRunProperties styleRunProperties1 = new();
    //    Bold bold1 = new();
    //    Color color1 = new() { ThemeColor = ThemeColorValues.Accent2 };
    //    RunFonts font1 = new() { Ascii = "Lucida Console" };
    //    Italic italic1 = new();
    //    // Specify a 12 point size.
    //    FontSize fontSize1 = new() { Val = "24" };
    //    styleRunProperties1.Append(bold1);
    //    styleRunProperties1.Append(color1);
    //    styleRunProperties1.Append(font1);
    //    styleRunProperties1.Append(fontSize1);
    //    styleRunProperties1.Append(italic1);

    //    // Add the run properties to the style.
    //    style.Append(styleRunProperties1);

    //    // Add the style to the styles part.
    //    styles?.Append(style);
    //}

    //// Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    //public static StyleDefinitionsPart? AddStylesPartToPackage(Document doc)
    //{
    //    if (doc.MainDocumentPart is null) return null;

    //    StyleDefinitionsPart styleDefinitionPart = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
    //    Styles root = new();
    //    root.Save(styleDefinitionPart);
    //    return styleDefinitionPart;
    //}


    //public static OpenXmlElement AddHeaderLevel(int level, string header, bool legalOption = false) => Header(new OpenXmlElement[] { new Run(new Text(header)) }, new OpenXmlElement[] { new Run(new Text(header)) }, level, legalOption);

    
    
    
}
