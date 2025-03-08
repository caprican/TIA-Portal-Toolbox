using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

//internal class AutolinkInlineRenderer : OpenXmlObjectRenderer<AutolinkInline>
//{
//    private int _hyperlinkIdCounter;

//    protected override void WriteObject(OpenXmlRenderer renderer, AutolinkInline obj)
//    {
//        var uriString = obj.Url;
//        var title = uriString;

//        if (obj.IsEmail && !uriString.ToLower().StartsWith("mailto:"))
//        {
//            uriString = "mailto:" + uriString;
//        }

//        var isAbsoluteUri = Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri);

//        if (!isAbsoluteUri)
//        {
//            Uri.TryCreate(uriString, UriKind.Relative, out uri);
//        }

//        if (uri is null) return;

//        var linkId = $"AL{_hyperlinkIdCounter++}";

//        //renderer.Document.MainDocumentPart?.AddHyperlinkRelationship(uri, isAbsoluteUri, linkId);
//        //var hl = new Hyperlink { Id = linkId };

//        //renderer.Cursor.Write(hl);
//        //renderer.Cursor.GoInto(hl);

//        //WriteText(renderer, title, renderer.Styles.Hyperlink);

//        //renderer.Cursor.PopAndAdvanceAfter(hl);
//    }
//}