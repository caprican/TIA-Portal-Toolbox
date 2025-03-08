using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

//internal class LinkInlineRenderer : OpenXmlObjectRenderer<LinkInline>
//{
//    private int _hyperlinkIdCounter = 1;

//    protected override void WriteObject(OpenXmlRenderer renderer, LinkInline obj)
//    {
//        Uri? uri = null;

//        var isAbsoluteUri = Uri.TryCreate(obj.Url, UriKind.Absolute, out uri);

//        if (!isAbsoluteUri)
//        {
//            Uri.TryCreate(obj.Url, UriKind.Relative, out uri);
//        }

//        //if (uri is null)
//        //{
//        //    renderer.WriteChildren(obj);
//        //}
//        //else
//        //{
//        //    var linkId = $"L{_hyperlinkIdCounter++}";

//        //    renderer.Document.MainDocumentPart?.AddHyperlinkRelationship(uri, isAbsoluteUri, linkId);
//        //    var hl = new Hyperlink()
//        //    {
//        //        Id = linkId,
//        //    };
//        //    renderer.Cursor.Write(hl);
//        //    renderer.Cursor.GoInto(hl);

//        //    renderer.TextStyle.Push(renderer.Styles.Hyperlink);
//        //    renderer.WriteChildren(obj);
//        //    renderer.TextStyle.Pop();

//        //    renderer.Cursor.PopAndAdvanceAfter(hl);
//        //}
//    }
//}