using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax;

using TiaPortalToolbox.Doc.Helpers;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

internal abstract class ParagraphRendererBase<T> : OpenXmlObjectRenderer<T> where T : Block
{
    protected Paragraph WriteAsParagraph(OpenXmlRenderer renderer, T obj, OpenXmlStyles? styleId)
    {
        var paragraph = new Paragraph();
        paragraph.SetStyle(styleId);

        //if (renderer.NoParagraph == 0)
        //{
        //    renderer.Cursor.Write(p);
        //    renderer.Cursor.GoInto(p);
        //}

        //renderer.NoParagraph++;

        RenderContents(renderer, obj);

        ///* Paragraph has been closed by somebody else during render (for example, nested list item) - let them handle
        // the consequences */
        //if (renderer.NoParagraph == 0) return p;

        //renderer.NoParagraph--;

        //if (renderer.NoParagraph == 0)
        //{
        //    renderer.Cursor.PopAndAdvanceAfter(p);
        //}

        return paragraph;
    }

    protected abstract void RenderContents(OpenXmlRenderer renderer, T block);
}