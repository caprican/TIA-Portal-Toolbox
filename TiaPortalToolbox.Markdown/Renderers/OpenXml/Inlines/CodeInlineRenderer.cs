using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax.Inlines;

using TiaPortalToolbox.Doc.Helpers;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

internal class CodeInlineRenderer : OpenXmlObjectRenderer<CodeInline>
{
    protected override void Write(OpenXmlRenderer renderer, CodeInline obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var run = new Run(new Text(obj.Content));
        run.SetStyle(OpenXmlStyles.Code);
        renderer.WriteInline(run);
    }
}