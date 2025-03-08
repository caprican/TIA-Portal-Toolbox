using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

//internal class ThematicBreakRenderer : LeafBlockParagraphRendererBase<ThematicBreakBlock>
//{
//    protected override void WriteObject(OpenXmlRenderer renderer, ThematicBreakBlock obj)
//    {
//        var p = WriteAsParagraph(renderer, obj, OpenXmlStyles.Document);
//        if (!p.Elements<Run>().Any())
//        {
//            p.AppendChild(new Run(new Text("")));
//        }
//    }
//}