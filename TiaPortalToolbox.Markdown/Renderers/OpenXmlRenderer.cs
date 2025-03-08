using System.Runtime.CompilerServices;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Helpers;
using Markdig.Syntax;

namespace TiaPortalToolbox.Doc.Renderers;

internal class OpenXmlRenderer : Markdig.Renderers.RendererBase
{
    private readonly Stack<OpenXmlElement> stack = new Stack<OpenXmlElement>();
    private char[]? buffer;

    private List<OpenXmlElement> Elements { get; } = [];

    public OpenXmlRenderer()
    {
        LoadRenderer();
    }

    public override object Render(MarkdownObject markdownObject)
    {
        Write(markdownObject);
        return Elements;
    }

    /// <summary>
    /// Loads the renderer used for render openXml
    /// </summary>
    protected virtual void LoadRenderer()
    {
        // Default block renderers
        //ObjectRenderers.Add(new Blocks.CodeBlockRenderer());
        //ObjectRenderers.Add(new Blocks.ListRenderer());
        //ObjectRenderers.Add(new Blocks.HeadingRenderer());
        ////ObjectRenderers.Add(new Blocks.HtmlBlockRenderer());
        ////ObjectRenderers.Add(new Blocks.HtmlRenderer());
        ObjectRenderers.Add(new OpenXml.Blocks.ParagraphRenderer());
        ObjectRenderers.Add(new OpenXml.Blocks.ListItemRenderer());
        //ObjectRenderers.Add(new Blocks.QuoteBlockRenderer());
        //ObjectRenderers.Add(new Blocks.ThematicBreakRenderer());

        // Default inline renderers
        //ObjectRenderers.Add(new Inlines.AutolinkInlineRenderer());
        ObjectRenderers.Add(new OpenXml.Inlines.CodeInlineRenderer());
        ObjectRenderers.Add(new OpenXml.Inlines.DelimiterInlineRenderer());
        ObjectRenderers.Add(new OpenXml.Inlines.EmphasisInlineRenderer());
        ObjectRenderers.Add(new OpenXml.Inlines.LineBreakInlineRenderer());
        ////ObjectRenderers.Add(new HtmlInlineRenderer());
        //ObjectRenderers.Add(new Inlines.HtmlEntityInlineRenderer());
        //ObjectRenderers.Add(new Inlines.LinkInlineRenderer());
        ObjectRenderers.Add(new OpenXml.Inlines.LiteralInlineRenderer());

        // Extension renderers
    }

    /// <summary>
    /// Writes the inlines of a leaf inline.
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <returns>This instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLeafInline(LeafBlock leafBlock)
    {
        if (leafBlock is null) throw new ArgumentNullException(nameof(leafBlock));
        var inline = (Markdig.Syntax.Inlines.Inline?)leafBlock.Inline;
        while (inline is not null)
        {
            Write(inline);
            inline = inline.NextSibling;
        }
    }

    /// <summary>
    /// Writes the lines of a <see cref="LeafBlock"/>
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    public void WriteLeafRawLines(LeafBlock leafBlock)
    {
        if (leafBlock is null) throw new ArgumentNullException(nameof(leafBlock));
        if (leafBlock.Lines.Lines is not null)
        {
            //stack.Push(new Run());

            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (var i = 0; i < lines.Count; i++)
            {
                if (i != 0)
                    WriteInline(new Break());

                WriteText(ref slices[i].Slice);
            }
        }
    }

    public void Push(OpenXmlElement o)
    {
        stack.Push(o);
    }

    public void Pop()
    {
        var popped = stack.Pop();
        if(stack.Count > 0)
        {
            stack.Peek().AppendChild(popped);
        }
        else
        {
            Elements.Add(popped);
        }
    }

    //public void WriteBlock(Block block)
    //{
    //    //stack.Peek().AddChild(block);
    //}

    public void WriteInline(OpenXmlElement inline)
    {
        AddInline(stack.Peek(), inline);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteText(ref StringSlice slice)
    {
        if (slice.Start > slice.End) return;
        WriteText(slice.Text, slice.Start, slice.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteText(string? text)
    {
        if(string.IsNullOrEmpty(text)) return;
        WriteInline(new Run(new Text(text!) { Space = SpaceProcessingModeValues.Preserve }));
    }

    public void WriteText(string? text, int offset, int length)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (offset == 0 && text!.Length == length)
        {
            WriteText(text);
        }
        else
        {
            if (buffer is null || length > buffer.Length)
            {
                buffer = text!.ToCharArray();
                WriteText(new string(buffer, offset, length));
            }
            else
            {
                text!.CopyTo(offset, buffer, 0, length);
                WriteText(new string(buffer, 0, length));
            }
        }
    }

    private static void AddInline(OpenXmlElement parent, OpenXmlElement inline)
    {
        parent.AppendChild(inline);
    }
}
