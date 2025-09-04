using System.Globalization;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using TiaPortalOpenness.Models;

namespace TiaPortalToolbox.Doc.Builders;

internal class PlcConstantChapterBuilder(Models.DocumentSettings settings, WordprocessingDocument document)
{
    private readonly Models.DocumentSettings settings = settings;

    private int IdentifierColumnSize = 1418;
    private int DescriptionColumnSize;

    private Markdig.Renderers.Docx.DocumentStyleTable? tableStyle;

    public Table Build(TiaPortalOpenness.Models.ProjectTree.Plc.Tag constant)
    {
        //DescriptionColumnSize = tableStyle.TableSize - IdentifierColumnSize - settings.DataTypeColumnSize - settings.DefaultValueColumnSize;
        tableStyle = settings.DocumentStyle.TableStyles["Default"] as Markdig.Renderers.Docx.DocumentStyleTable;
        List<int> columns = [IdentifierColumnSize, DescriptionColumnSize];

        var table = new Table().CreateTable(columns, tableStyle!.StyleName, indentation:113 );
        //table.Append(Helpers.DocumentHelper.CreateHeaderTable(
        //    [
        //        new("Identifier & Value", IdentifierColumnSize),
        //        new("Description", DescriptionColumnSize)
        //    ], settings));

        //if (constant.Members?[culture] is not null)
        //{
        //    table.Append(BuildCorp(constant.Members[culture], culture));
        //}

        return table;
    }

    private TableRow[] BuildCorp(List<InterfaceMember> members, CultureInfo culture)
    {
        var rows = new List<TableRow>();
        foreach (var member in members)
        {
            rows.Add(AddRow(member, culture, member == members.Last()));
            if (member.Members?.Count() > 0)
            {
                rows.AddRange(BuildCorp(member.Members, culture));
            }
        }

        return [.. rows];
    }

    private TableRow AddRow(InterfaceMember member, CultureInfo culture, bool isLastRow)
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties(new CantSplit())
        };

        var tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = "0", Type = TableWidthUnitValues.Auto },
                Shading = new Shading { Color = tableStyle!.ShadingColor, Fill = tableStyle.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
            }
        };
        var paragraph = new Paragraph(new Run(new Text(member.Name!))
        {
            RunProperties = new RunProperties
            {
                Bold = new Bold { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
            }
        })
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                KeepNext = new KeepNext()
            }
        };
        tableCell.Append(paragraph);

        paragraph = new Paragraph(new Run(new Text(member.DefaultValue))
        {
            RunProperties = new RunProperties
            {
                NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
            }
        })
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                KeepNext = new KeepNext()
            }
        };
        tableCell.Append(paragraph);

        tableRow.Append(tableCell);

        tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{DescriptionColumnSize}", Type = TableWidthUnitValues.Auto },
                Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.ShadingFill, Val = ShadingPatternValues.Clear }
            }
        };

        if (!string.IsNullOrEmpty(member.Descriptions?[culture]))
        {

            //if (Helpers.DocumentHelper.MarkdownToParagraph(member.Descriptions![culture], opennessService) is IEnumerable<OpenXmlElement> xElements)
            //{
            //    tableCell.Append(xElements);
            //}
        }
        else
        {
            tableCell.Append(new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                    KeepNext = new KeepNext()
                }
            });
        }

        tableRow.Append(tableCell);

        return tableRow;
    }
}
