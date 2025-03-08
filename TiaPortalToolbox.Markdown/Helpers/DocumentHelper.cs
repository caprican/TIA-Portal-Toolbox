using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using TiaPortalToolbox.Doc.Models;
using System.Globalization;
using Markdig;
using TiaPortalToolbox.Doc.Renderers;

namespace TiaPortalToolbox.Doc.Helpers;

internal static class DocumentHelper
{
    public static readonly string NS_WORD12 = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    public static readonly string W_NAMESPACE_DECLARATION = "xmlns:w=\"" + NS_WORD12 + "\"";

    internal static Dictionary<OpenXmlStyles, (string Name, int? Level)> Styles = [];

    internal static Table CreateTable(int rows, int cols, string? styleId = null)
    {
        var table = new Table(new TableProperties
        {
            TableStyle = new TableStyle { Val = styleId ?? "TableGrid" },
            TableWidth = new TableWidth { Type = TableWidthUnitValues.Auto, Width = "0" },
            TableLook = new TableLook { Val = "04A0" }
        });

        var tblGrid = new TableGrid();
        table.AppendChild(tblGrid);

        for (var row = 0; row < rows; row++)
        {
            var tr = new TableRow();
            table.AppendChild(tr);
            for (var cell = 0; cell < cols; cell++)
            {
                var tc = new TableCell();
                tr.AppendChild(tc);

                tc.AppendChild(new Run(new Text("")));
            }
        }

        return table;
    }

    internal static Table CreateTable(int width, int indentation, List<int> columns)
    {
        var table = new Table(new TableProperties
        {
            TableStyle = new TableStyle { Val = Styles[OpenXmlStyles.TableGrid].Name },
            TableWidth = new TableWidth { Width = $"{width}", Type = TableWidthUnitValues.Dxa },
            TableIndentation = new TableIndentation { Width = indentation, Type = TableWidthUnitValues.Dxa },
            TableLook = new TableLook { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true }
        });

        var tableGrid = new TableGrid();
        foreach (var column in columns)
        {
            tableGrid.Append(new GridColumn { Width = $"{column}" });
        }
        table.Append(tableGrid);
        return table;
    }

    internal static TableRow CreateHeaderTable(List<Models.TableHeader> headers, DocumentSettings settings)
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties(new CantSplit(), new DocumentFormat.OpenXml.Wordprocessing.TableHeader())
        };

        foreach (var header in headers)
        {
            var tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{header.Width}", Type = TableWidthUnitValues.Dxa },
                    GridSpan = header.GridSpan > 0 ? new GridSpan { Val = header.GridSpan } : null,
                    TableCellBorders = new TableCellBorders
                    {
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderHeaderSize, Color = settings.BorderColor }
                    },
                    Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFillHeader, Val = ShadingPatternValues.Clear },
                    NoWrap = new NoWrap(),
                    TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                }
            };
            var paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Styles[OpenXmlStyles.TableTextLeft].Name }
                }
            };
            paragraph1.Append(new Run(new Text(header.Title)));
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);
        }
        return tableRow;
    }

    public static void SetCellBorder(this TableCell tc, CellBorderType borderType, BorderValues? border)
    {
        var tcPr = tc.TableCellProperties;

        if (tcPr is null)
        {
            tcPr = new TableCellProperties();
            tc.TableCellProperties = tcPr;
        }

        var tcBorders = tcPr.TableCellBorders;
        if (tcBorders is null)
        {
            tcBorders = new TableCellBorders();
            tcPr.TableCellBorders = tcBorders;
        }

        switch (borderType)
        {
            case CellBorderType.Left:
                tcBorders.LeftBorder = new LeftBorder() { Val = border };
                break;
            case CellBorderType.Right:
                tcBorders.RightBorder = new RightBorder() { Val = border };
                break;
            case CellBorderType.Top:
                tcBorders.TopBorder = new TopBorder() { Val = border };
                break;
            case CellBorderType.Bottom:
                tcBorders.BottomBorder = new BottomBorder() { Val = border };
                break;
        }
    }

    public static void AddHorizontalSpan(this TableRow row, int span)
    {
        var firstCell = row.ChildElements
            .OfType<TableCell>()
            .FirstOrDefault();

        if (firstCell is null) return;

        var tcPr = firstCell.TableCellProperties;
        if (tcPr is null)
        {
            tcPr = new TableCellProperties();
            firstCell.TableCellProperties = tcPr;
        }

        var gridSpan = tcPr.GridSpan;
        if (gridSpan is null)
        {
            gridSpan = new GridSpan();
            tcPr.GridSpan = gridSpan;
        }

        gridSpan.Val = span;
    }

    public static TableRow AddRow(this Table table, bool isHeader, params string[] values) => AddRow(table, isHeader, values, null);

    internal static TableRow AddRow(this Table table, bool isHeader, string[] values, OpenXmlStyles[]? styles)
    {
        var row = new TableRow();
        table.AppendChild(row);
        if (isHeader)
        {
            row.TableRowProperties = new TableRowProperties(new DocumentFormat.OpenXml.Wordprocessing.TableHeader());
        }

        for (var i = 0; i < values.Length; i++)
        {
            var val = values[i];
            var tc = new TableCell();
            row.AppendChild(tc);

            var text = CreateParagraph(val);
            tc.AppendChild(text);

            if (styles?.Count() > 0)
            {
                SetStyle(text, styles[i]);
            }
        }

        return row;
    }

    public static Paragraph CreateParagraph(string text) => new(new Run(new Text(text)));

    public static void InsertAfterLastOfType<T>(this OpenXmlCompositeElement parent, OpenXmlElement element) where T : OpenXmlElement
    {
        var refElement = parent.Elements<T>().LastOrDefault();
        if (refElement is null)
        {
            parent.AppendChild(element);
        }
        else
        {
            parent.InsertAfter(element, refElement);
        }
    }

    public static void InsertAfterLastOfType(this OpenXmlCompositeElement parent, OpenXmlElement element)
    {
        var refElement = parent.Elements().LastOrDefault(e => e.GetType() == element.GetType());
        if (refElement is null)
        {
            parent.AppendChild(element);
        }
        else
        {
            parent.InsertAfter(element, refElement);
        }
    }

    public static void SetStyle(this Run run, OpenXmlStyles? styleId)
    {
        if (styleId is null) return;

        run.RunProperties ??= new RunProperties();
        run.RunProperties.RunStyle ??= new RunStyle();
        run.RunProperties.RunStyle.Val = Styles[styleId.Value].Name;
    }

    public static AbstractNum AddOrderedListAbstractNumbering(this WordprocessingDocument document)
    {
        var numbering = document.GetOrCreateNumbering();
        var abstractNumId = numbering?.Elements<AbstractNum>().Count() + 1;

        var abstractNum = new AbstractNum(
            new Level(
                new NumberingFormat() { Val = NumberFormatValues.Decimal },
                new LevelText() { Val = "%1." }
            )
            { LevelIndex = 0, StartNumberingValue = new StartNumberingValue() { Val = 1 } }
        )
        {
            AbstractNumberId = abstractNumId,
            MultiLevelType = new MultiLevelType { Val = MultiLevelValues.SingleLevel }
        };

        numbering?.AddAbstractNumbering(abstractNum);

        return abstractNum;
    }

    public static AbstractNum AddBulletListAbstractNumbering(this WordprocessingDocument document)
    {
        var numbering = document.GetOrCreateNumbering();
        var abstractNumberId = numbering?.Elements<AbstractNum>().Count() + 1;

        var abstractNum = new AbstractNum(
            new Level(
                new NumberingFormat() { Val = NumberFormatValues.Bullet },
                new LevelText() { Val = "·" }
            ) { LevelIndex = 0 }
        ) { AbstractNumberId = abstractNumberId };

        numbering?.AddAbstractNumbering(abstractNum);
        return abstractNum;
    }

    public static NumberingInstance AddOrderedListNumbering(this WordprocessingDocument document, int abstractNumId, int? startFrom = null)
    {
        var numbering = document.GetOrCreateNumbering();
        var numId = numbering?.Elements<NumberingInstance>().Count() + 1;
        var numberingInstance = new NumberingInstance(new AbstractNumId() { Val = abstractNumId }) { NumberID = numId };
        numbering?.AddNumberingInstance(numberingInstance);

        if (startFrom is not null)
        {
            numberingInstance.AppendChild(new LevelOverride
            {
                LevelIndex = 0,
                StartOverrideNumberingValue = new StartOverrideNumberingValue() { Val = startFrom }
            });
        }

        return numberingInstance;
    }

    public static NumberingInstance AddBulletedListNumbering(this WordprocessingDocument document, AbstractNum? abstractNum = null)
    {
        var numbering = document.GetOrCreateNumbering();
        if (abstractNum is null)
        {
            var abstractNumberId = numbering?.Elements<AbstractNum>().Count() + 1;

            abstractNum = new AbstractNum(
                new Level(
                    new NumberingFormat() { Val = NumberFormatValues.Bullet },
                    new LevelText() { Val = "·" }
                )
                { LevelIndex = 0 }
            )
            { AbstractNumberId = abstractNumberId };

            numbering?.AddAbstractNumbering(abstractNum);
        }

        var numId = numbering?.Elements<NumberingInstance>().Count() + 1;
        var numberingInstance = new NumberingInstance(new AbstractNumId() { Val = abstractNum.AbstractNumberId }) { NumberID = numId };
        numbering?.AddNumberingInstance(numberingInstance);

        return numberingInstance;
    }

    public static void AddAbstractNumbering(this Numbering numbering, AbstractNum abstractNum)
    {
        numbering.InsertAfterLastOfType(abstractNum);
        numbering.Save();
    }

    public static void AddNumberingInstance(this Numbering numbering, NumberingInstance numberingInstance)
    {
        numbering.InsertAfterLastOfType(numberingInstance);
        numbering.Save();
    }

    public static Numbering? GetOrCreateNumbering(this WordprocessingDocument document)
    {
        if (document.MainDocumentPart?.NumberingDefinitionsPart is null)
        {
            var part = document.MainDocumentPart?.AddNewPart<NumberingDefinitionsPart>();
            if(part is not null)
            {
                part.Numbering = new Numbering();
            }
        }

        var numbering = document.MainDocumentPart?.NumberingDefinitionsPart!.Numbering;
        return numbering;
    }

    public static ParagraphProperties GetOrCreateProperties(this Paragraph paragraph)
    {
        paragraph.ParagraphProperties ??= new ParagraphProperties();
        return paragraph.ParagraphProperties;
    }

    public static RunProperties GetOrCreateProperties(this Run run)
    {
        run.RunProperties ??= new RunProperties();
        return run.RunProperties;
    }

    public static void SetStyle(this Paragraph? paragraph, OpenXmlStyles? styleId)
    {
        if (paragraph is null || styleId is null) return;

        paragraph.ParagraphProperties ??= new ParagraphProperties();
        paragraph.ParagraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = Styles[styleId.Value].Name };
    }

    //public static Paragraph AddStyledParagraphOfText(this MainDocumentPart mainDocumentPart, string styleId, string text)
    //{
    //    var paragraph = CreateParagraphOfText(text);

    //    mainDocumentPart.Document.Body ??= new Body();
    //    mainDocumentPart.Document.Body!.AppendChild(paragraph);

    //    mainDocumentPart.Document.ApplyStyleToParagraph(styleId, paragraph);

    //    return paragraph;
    //}

    public static Paragraph CreateParagraphOfText(string? simpleText)
    {
        var paragraph = new Paragraph();
        if (simpleText is null) return paragraph;

        var afterNewline = false;
        var run = new Run();
        foreach (var s in simpleText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
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

            run.AppendChild(t); // ContentAccessor					
            afterNewline = true;
        }

        paragraph.AppendChild(run); // ContentAccessor
        return paragraph;
    }

    //// Apply a style to a paragraph.
    //public static void ApplyStyleToParagraph(this Document doc, string styleid, Paragraph p)
    //{
    //    // If the paragraph has no ParagraphProperties object, create one.
    //    if (!p.Elements<ParagraphProperties>().Any())
    //    {
    //        p.PrependChild<ParagraphProperties>(new ParagraphProperties());
    //    }

    //    // Get the paragraph properties element of the paragraph.
    //    var pPr = p.Elements<ParagraphProperties>().First();

    //    // Get the Styles part for this document.
    //    var styleDefinitionPart = doc.MainDocumentPart!.StyleDefinitionsPart;

    //    // If the Styles part does not exist, add it and then add the style.
    //    if (styleDefinitionPart is null)
    //    {
    //        styleDefinitionPart = AddStylesPartToPackage(doc);
    //        if(styleDefinitionPart is not null) AddNewStyle(styleDefinitionPart, styleid, styleid);
    //    }
    //    else
    //    {
    //        // If the style is not in the document, add it.
    //        if (IsStyleIdInDocument(doc, styleid) != true)
    //        {
    //            // No match on styleid, so let's try style name.
    //            var styleidFromName = GetStyleIdFromStyleName(doc, styleid);
    //            if (styleidFromName is null)
    //            {
    //                AddNewStyle(styleDefinitionPart, styleid, styleid);
    //            }
    //            else
    //                styleid = styleidFromName;
    //        }
    //    }

    //    // Set the style of the paragraph.
    //    pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
    //}

    // Return true if the style id is in the document, false otherwise.
    public static bool IsStyleIdInDocument(Document doc, string styleid)
    {
        // Get access to the Styles element for this document.
        var s = doc.MainDocumentPart?.StyleDefinitionsPart?.Styles;

        // Check that there are styles and how many.
        var n = s?.Elements<Style>().Count();
        if (n == 0) return false;

        // Look for a match on styleid.
        var style = s?
            .Elements<Style>()
            .FirstOrDefault(st => st.Type != null && (st.StyleId == styleid) && st.Type == StyleValues.Paragraph);

        return style is not null;
    }

    // Return styleid that matches the styleName, or null when there's no match.
    public static string? GetStyleIdFromStyleName(Document doc, string styleName)
    {
        var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
        var styleId = stylePart?.Styles?.Descendants<StyleName>()
            .Where(s => styleName.Equals(s.Val?.Value)
                        && s.Parent is Style parent
                        && parent.Type != null
                        && parent.Type == StyleValues.Paragraph)
            .Select(n => n.Parent as Style)
            .Where(n => n is not null)
            .Select(n => n!.StyleId).FirstOrDefault();

        return styleId;
    }

    //// Create a new style with the specified styleid and stylename and add it to the specified
    //// style definitions part.
    //private static void AddNewStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename)
    //{
    //    // Get access to the root element of the styles part.
    //    Styles? styles = styleDefinitionsPart.Styles;

    //    // Create a new paragraph style and specify some of the properties.
    //    Style style = new()
    //    {
    //        Type = StyleValues.Paragraph,
    //        StyleId = styleid,
    //        CustomStyle = true
    //    };
    //    StyleName styleName1 = new() { Val = stylename };
    //    BasedOn basedOn1 = new() { Val = "Normal" };
    //    NextParagraphStyle nextParagraphStyle1 = new() { Val = "Normal" };
    //    style.Append(styleName1);
    //    style.Append(basedOn1);
    //    style.Append(nextParagraphStyle1);

    //    // Create the StyleRunProperties object and specify some of the run properties.
    //    StyleRunProperties styleRunProperties1 = new();
    //    Bold bold1 = new();
    //    Color color1 = new() { ThemeColor = ThemeColorValues.Accent2 };
    //    RunFonts font1 = new() { Ascii = "Lucida Console" };
    //    Italic italic1 = new();
    //    // Specify a 12 point size.
    //    FontSize fontSize1 = new() { Val = "24" };
    //    styleRunProperties1.Append(bold1);
    //    styleRunProperties1.Append(color1);
    //    styleRunProperties1.Append(font1);
    //    styleRunProperties1.Append(fontSize1);
    //    styleRunProperties1.Append(italic1);

    //    // Add the run properties to the style.
    //    style.Append(styleRunProperties1);

    //    // Add the style to the styles part.
    //    styles?.Append(style);
    //}

    //// Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    //public static StyleDefinitionsPart? AddStylesPartToPackage(Document doc)
    //{
    //    if (doc.MainDocumentPart is null) return null;

    //    StyleDefinitionsPart styleDefinitionPart = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
    //    Styles root = new();
    //    root.Save(styleDefinitionPart);
    //    return styleDefinitionPart;
    //}


    //public static OpenXmlElement AddHeaderLevel(int level, string header, bool legalOption = false) => Header(new OpenXmlElement[] { new Run(new Text(header)) }, new OpenXmlElement[] { new Run(new Text(header)) }, level, legalOption);
    
    // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    internal static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument? doc, CultureInfo culture)
    {
        StyleDefinitionsPart part;

        if (doc?.MainDocumentPart is null)
        {
            throw new ArgumentNullException("MainDocumentPart is null.");
        }

        part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
        part.Styles = new DocumentFormat.OpenXml.Wordprocessing.Styles();

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

    internal static NumberingDefinitionsPart AddNumberingDefinitionsPartToPackage(WordprocessingDocument? doc, CultureInfo culture)
    {
        NumberingDefinitionsPart part;
        if (doc?.MainDocumentPart is null)
        {
            throw new ArgumentNullException("MainDocumentPart is null.");
        }

        part = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();
        part.Numbering = new DocumentFormat.OpenXml.Wordprocessing.Numbering();

        return part;
    }

    internal static object MarkdownToParagraph(string markdownString)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Use<MarkdownExtensions>()
            .Build();

        // Run the convertion
        return Markdown.Convert(markdownString, new OpenXmlRenderer(), pipeline);
    }
}
