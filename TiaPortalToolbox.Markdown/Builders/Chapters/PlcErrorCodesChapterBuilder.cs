using System.Globalization;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Builders;

internal class PlcErrorCodesChapterBuilder(Models.DocumentSettings settings, WordprocessingDocument document)
{
    private readonly Models.DocumentSettings settings = settings;

    private Markdig.Renderers.Docx.DocumentStyleTable? tableStyle;

    private int IdentifierColumnSize = 1418;
    private int DescriptionColumnSize;

    public Table Build(Core.Models.ProjectTree.Plc.Tag tag)
    {
        tableStyle = settings.DocumentStyle.TableStyles["Default"] as Markdig.Renderers.Docx.DocumentStyleTable;

        //DescriptionColumnSize = settings.TableSize - IdentifierColumnSize - settings.DataTypeColumnSize - settings.DefaultValueColumnSize;

        List<int> columns = [IdentifierColumnSize, DescriptionColumnSize];

        var table = new Table().CreateTable(columns, styleName: tableStyle.StyleName, indentation: 113 );
        //table.Append(Helpers.DocumentHelper.CreateHeaderTable(
        //    [
        //        new("Code / Value", IdentifierColumnSize),
        //        new("Identifier / Description", DescriptionColumnSize)
        //    ], settings));

        //if (tag.Members?[culture] is not null)
        //{
        //    table.Append(BuildCorp(tag.Members[culture], culture));
        //}

        return table;

    }

    private TableRow[] BuildCorp(List<Core.Models.InterfaceMember> members, CultureInfo culture)
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

    private TableRow AddRow(Core.Models.InterfaceMember member, CultureInfo culture, bool isLastRow)
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
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
            }
        };
        var paragraph = new Paragraph(new Run(new Text(member.Name!))
        {
            RunProperties = new RunProperties
            {
                NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
            }
        })
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle!.StyleTextLeft },
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
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
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
