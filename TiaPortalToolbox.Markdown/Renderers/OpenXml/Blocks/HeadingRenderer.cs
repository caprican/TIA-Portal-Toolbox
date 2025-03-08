using Markdig.Syntax;

using TiaPortalToolbox.Doc.Helpers;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

//internal class HeadingRenderer : LeafBlockParagraphRendererBase<HeadingBlock>
//{
//    protected override void WriteObject(OpenXmlRenderer renderer, HeadingBlock obj)
//    {
//        var style = obj.Level switch
//        {
//            0 => OpenXmlStyles.Title,
//            1 => OpenXmlStyles.Heading1,
//            2 => OpenXmlStyles.Heading2,
//            3 => OpenXmlStyles.Heading3,
//            4 => OpenXmlStyles.Heading4,
//            5 => OpenXmlStyles.Heading5,
//            6 => OpenXmlStyles.Heading6,
//            _ => OpenXmlStyles.BlockText,
//        };

//        WriteAsParagraph(renderer, obj, style);
//    }
//}