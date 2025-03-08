using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

internal class LiteralInlineRenderer : OpenXmlObjectRenderer<LiteralInline>
{
    protected override void Write(OpenXmlRenderer renderer, LiteralInline obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        if (obj.Content.IsEmpty)
            return;

        renderer.WriteText(ref obj.Content);
    }
}