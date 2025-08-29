using System.Globalization;

using DocumentFormat.OpenXml.Wordprocessing;

using Markdig;

namespace DocumentFormat.OpenXml.Packaging;

public static class WordprocessingDocumentExtensions
{
    public static void BodyAppend(this WordprocessingDocument document, params OpenXmlElement[] newChildren)
    {
        document.MainDocumentPart?.Document.Body?.Append(newChildren);
    }

    // Add a StylesDefinitionsPart to the document. Returns a reference to it.
    public static StyleDefinitionsPart AddStylesPartToPackage(this WordprocessingDocument doc, CultureInfo culture)
    {
        StyleDefinitionsPart part;

        if (doc?.MainDocumentPart is null)
        {
            throw new ArgumentNullException("MainDocumentPart is null.");
        }

        part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
        part.Styles = new Styles();

        part.Styles.Append(new DocDefaults
        {
            RunPropertiesDefault = new RunPropertiesDefault
            {
                RunPropertiesBaseStyle = new RunPropertiesBaseStyle
                {
                    RunFonts = new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", EastAsia = "Times New Roman", ComplexScript = "Times New Roman" },
                    Languages = new Languages { Val = culture.Name, EastAsia = culture.Name }
                }
            },
            ParagraphPropertiesDefault = new ParagraphPropertiesDefault()
        });

        return part;
    }

    public static NumberingDefinitionsPart AddNumberingDefinitionsPartToPackage(this WordprocessingDocument doc, CultureInfo culture)
    {
        NumberingDefinitionsPart part;
        if (doc?.MainDocumentPart is null)
        {
            throw new ArgumentNullException("MainDocumentPart is null.");
        }

        part = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();
        part.Numbering = new Numbering();

        return part;
    }

    public static void MarkdownConvert(this WordprocessingDocument document, TiaPortalToolbox.Doc.Models.DocumentSettings settings, string markdownString)
    {
        var renderer = new Markdig.Renderers.Docx.DocxDocumentRenderer(document, settings.DocumentStyle)
        {
            ImagesBaseUri = settings.UserFolderPath,
            LinksBaseUri = "",
            SkipImages = false,
        };

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions()
                                                    .UseEmojiAndSmiley()
                                                    .Build();

        var markdownDocument = Markdown.Parse(markdownString, pipeline);

        renderer.Render(markdownDocument);
    }
}
