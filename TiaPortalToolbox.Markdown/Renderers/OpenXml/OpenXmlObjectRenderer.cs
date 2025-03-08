using Markdig.Syntax;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml;

internal abstract class OpenXmlObjectRenderer<T> : Markdig.Renderers.MarkdownObjectRenderer<OpenXmlRenderer, T> where T : MarkdownObject
{

}
