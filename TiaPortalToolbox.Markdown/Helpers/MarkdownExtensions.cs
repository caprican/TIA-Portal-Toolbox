using System;
using System.Collections.Generic;
using System.Text;

using Markdig;
using Markdig.Renderers;

namespace TiaPortalToolbox.Doc.Helpers;

public class MarkdownExtensions : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        throw new NotImplementedException();
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        throw new NotImplementedException();
    }
}
