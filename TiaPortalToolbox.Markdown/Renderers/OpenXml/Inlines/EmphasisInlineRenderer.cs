using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax.Inlines;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Inlines;

internal class EmphasisInlineRenderer : OpenXmlObjectRenderer<EmphasisInline>
{
    protected override void Write(OpenXmlRenderer renderer, EmphasisInline obj)
    {
        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var runProperties = new RunProperties();

        switch (obj.DelimiterChar)
        {
            case '*':
            case '_':
                if (obj.DelimiterCount == 2)
                {
                    runProperties.Bold = new Bold();
                }
                else
                {
                    runProperties.Italic = new Italic();
                }
                break;
            case '~':
                if (obj.DelimiterCount == 2)
                {
                    runProperties.Strike = new Strike();
                }
                else
                {
                    runProperties.VerticalTextAlignment = new VerticalTextAlignment { Val = VerticalPositionValues.Subscript };
                }
                break;
            case '^':
                runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript };
                break;
            case '+':
                runProperties.Highlight = new Highlight { Val = HighlightColorValues.Green };
                break;
            case '=':
                runProperties.Highlight = new Highlight { Val = HighlightColorValues.Yellow };
                break;
            default:
                runProperties = null;
                break;
        }

        if (runProperties is not null)
        {
            renderer.Push(runProperties);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
        else
        {
            renderer.WriteChildren(obj);
        }
    }
}
