using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

internal class DelimiterInlineRenderer : OpenXmlObjectRenderer<DelimiterInline>
{
    protected override void Write(OpenXmlRenderer renderer, DelimiterInline obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        renderer.WriteText(obj.ToLiteral());
        renderer.WriteChildren(obj);
    }
}
