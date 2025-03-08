using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

//internal class CodeBlockRenderer : LeafBlockParagraphRendererBase<CodeBlock>
//{
//    protected override void WriteObject(OpenXmlRenderer renderer, CodeBlock obj)
//    {
//        WriteAsParagraph(renderer, obj, OpenXmlStyles.Code);
//    }

//    protected override void RenderContents(OpenXmlRenderer renderer, CodeBlock obj)
//    {
//        var lines = obj.Lines;

//        for (var i = 0; i < lines.Count; i++)
//        {
//            var line = lines.Lines[i];
//            var text = line.ToString() ?? "";

//            var run = new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
//            //renderer.Cursor.Write(run);

//            if (i < lines.Count - 1)
//            {
//                var breakRun = new Run(new Break());
//                //renderer.Cursor.Write(breakRun);
//            }
//        }
//    }
//}