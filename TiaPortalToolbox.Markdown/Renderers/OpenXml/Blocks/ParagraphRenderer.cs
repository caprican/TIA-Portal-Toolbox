using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax;

using TiaPortalToolbox.Doc.Helpers;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

internal class ParagraphRenderer : OpenXmlObjectRenderer<ParagraphBlock>
{
    protected override void Write(OpenXmlRenderer renderer, ParagraphBlock obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var paragraph = new Paragraph();
        paragraph.SetStyle(OpenXmlStyles.Document);

        renderer.Push(paragraph);
        renderer.Push(new Run());
        renderer.WriteLeafInline(obj);
        renderer.Pop();
        renderer.Pop();
    }
}
