using DocSharp.Helpers;

using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Wordprocessing;

public static class ParagraphExtensions
{
    public static bool IsFirst(this Paragraph paragraph) => paragraph.PreviousSibling<Paragraph>() is null;

    public static bool IsLast(this Paragraph paragraph) => paragraph.Parent is not null && paragraph.Parent.LastChild == paragraph;

    public static bool IsEmpty(this Paragraph paragraph) => paragraph.ChildElements.Count == 0 || (paragraph.ChildElements.Count == 1 && paragraph.ParagraphProperties != null);

    public static ParagraphProperties GetOrCreateProperties(this Paragraph p)
    {
        p.ParagraphProperties ??= new ParagraphProperties();
        return p.ParagraphProperties;
    }

    public static void SetStyle(this Paragraph? para, string? styleId)
    {
        if (para is null || styleId is null) return;

        var pPr = para.ParagraphProperties;
        if (pPr is null)
        {
            pPr = new ParagraphProperties();
            para.ParagraphProperties = pPr;
        }

        var style = new ParagraphStyleId() { Val = styleId };
        pPr.ParagraphStyleId = style;
    }

    public static Paragraph AddParagraph(this MainDocumentPart mainDocumentPart, string text, string? styleId)
    {
        var paragraph = CreateParagraph(text);

        mainDocumentPart.Document.Body ??= new Body();
        mainDocumentPart.Document.Body.AppendChild(paragraph);

        if (!string.IsNullOrEmpty(styleId))
        {
            mainDocumentPart.Document.ApplyStyleToParagraph(styleId!, paragraph);
        }
        return paragraph;
    }

    public static Paragraph CreateParagraph(string? text)
    {
        var para = new Paragraph();

        if (text is null) return para;

        var splits = text.NormalizeNewLines().Split('\n');

        var afterNewline = false;
        var run = new Run();
        foreach (var s in splits)
        {
            if (afterNewline)
            {
                var br = new Break();
                run.AppendChild(br);
            }

            Text t = new(s);

            if (s.StartsWith(" ") || s.EndsWith(" "))
            {
                t.Space = SpaceProcessingModeValues.Preserve;
            }

            run.AppendChild(t);
            afterNewline = true;
        }

        para.AppendChild(run);
        return para;
    }

    public static void ApplyStyleToParagraph(this Document doc, string styleid, Paragraph p)
    {
        if (doc.MainDocumentPart is not null)
        {
            // If the paragraph has no ParagraphProperties object, create one.
            var pPr = p.GetOrCreateProperties();

            // Get the Styles part for this document.
            var styles = doc.MainDocumentPart.GetOrCreateStylesPart();
            var style = styles.GetStyleFromId(styleid, StyleValues.Paragraph);
            if (style != null)
            {
                pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
            }
        }
    }

    public static Paragraph? FindParagraphContainingText(WordprocessingDocument document, string text)
    {
        if (document.MainDocumentPart is null || document.MainDocumentPart.Document.Body is null) return null;

        var textElement = document.MainDocumentPart.Document.Body
            .Descendants<Text>().FirstOrDefault(t => t.Text.Contains(text));

        if (textElement == null) return null;

        var p = textElement.Ancestors<Paragraph>().FirstOrDefault();
        return p;
    }
}
