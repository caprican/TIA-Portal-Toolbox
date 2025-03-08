using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

internal class LineBreakInlineRenderer : OpenXmlObjectRenderer<LineBreakInline>
{
    protected override void Write(OpenXmlRenderer renderer, LineBreakInline obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        if (obj.IsHard)
        {
            renderer.WriteInline(new Run(new Break()));
        }
        else
        {
            renderer.WriteText(" ");
        }
    }
}