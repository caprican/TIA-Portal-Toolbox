using Markdig;
using Markdig.Renderers;

namespace TiaPortalToolbox.Doc.Helpers;

internal class MarkdownExtensions : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        
    }
}
