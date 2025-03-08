using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Builders;

internal class UserDatatypeChapter(Models.DocumentSettings settings)
{
    private readonly Models.DocumentSettings settings = settings;

    private int IdentifierColumnSize = 1418;
    private int DescriptionColumnSize;

    public Table Build(Core.Models.ProjectTree.Plc.Type udt, CultureInfo culture)
    {
        var levelMax = GetLevelCount(udt.Members?[culture]);

        IdentifierColumnSize += (levelMax * settings.IdentColumnSize / 10);
        DescriptionColumnSize = settings.TableSize - IdentifierColumnSize - settings.DataTypeColumnSize - settings.DefaultValueColumnSize;

        var columns = Enumerable.Repeat(settings.IdentColumnSize, levelMax + 1).ToList();
        columns.Add(settings.DataTypeColumnSize);
        columns.Add(settings.DefaultValueColumnSize);
        columns.Add(DescriptionColumnSize);

        var table = Helpers.DocumentHelper.CreateTable(settings.TableSize, 113, columns);
        table.Append(Helpers.DocumentHelper.CreateHeaderTable(
            [
                new("Identifier", IdentifierColumnSize, levelMax + 1),
                new("Data type", settings.DataTypeColumnSize),
                new("Default value", settings.DefaultValueColumnSize),
                new("Description", DescriptionColumnSize)
            ], settings));

        if (udt.Members?[culture] is not null)
        {
            table.Append(BuildCorp(udt.Members[culture], culture, 0, levelMax));
        }

        return table;
    }

    private int GetLevelCount(List<Core.Models.InterfaceMember>? members)
    {
        if (members is null) return 0;

        var levelMax = 0;
        var defines = members;

        while (defines.Any(udt => udt.Members is not null))
        {
            defines = [.. defines.Where(udt => udt.Members is not null).SelectMany(s => s.Members)];
            levelMax++;
        }

        return levelMax;
    }

    private TableRow[] BuildCorp(List<Core.Models.InterfaceMember> members, CultureInfo culture, int level, int levelMax)
    {
        var rows = new List<TableRow>();
        foreach (var member in members)
        {
            rows.Add(AddRow(member, culture, level, levelMax, member == members.Last()));
            if (member.Members?.Count() > 0)
            {
                rows.AddRange(BuildCorp(member.Members, culture, level + 1, levelMax));
            }
        }

        return [.. rows];
    }

    private TableRow AddRow(Core.Models.InterfaceMember member, CultureInfo culture, int level, int levelMax, bool isLastRow)
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties(new CantSplit())
        };

        if (level > 0)
        {
            for (var i = 0; i < level; i++)
            {
                var indentCell = new TableCell
                {
                    TableCellProperties = new TableCellProperties
                    {
                        TableCellWidth = new TableCellWidth { Width = $"{settings.IdentColumnSize}", Type = TableWidthUnitValues.Dxa },
                        TableCellBorders = new TableCellBorders
                        {
                            TopBorder = new TopBorder { Val = BorderValues.Nil },
                            BottomBorder = isLastRow ? null : new BottomBorder { Val = BorderValues.Nil }
                        }
                    }
                };
                indentCell.Append(new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        KeepNext = new KeepNext()
                    }
                });

                tableRow.Append(indentCell);
            }
        }

        var tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = "0", Type = TableWidthUnitValues.Auto },
                Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
                GridSpan = new GridSpan { Val = levelMax - level + 1 },
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
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
                KeepNext = new KeepNext()
            }
        };
        tableCell.Append(paragraph);
        tableRow.Append(tableCell);

        tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{settings.DataTypeColumnSize}", Type = TableWidthUnitValues.Dxa },
                Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
            }
        };
        paragraph = new Paragraph(new Run(new Text(member.Type!))
        {
            RunProperties = new RunProperties
            {
                NoProof = new NoProof { Val = new OnOffValue(true) },
            }
        })
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
                KeepNext = new KeepNext()
            }
        };
        tableCell.Append(paragraph);
        tableRow.Append(tableCell);

        tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{settings.DefaultValueColumnSize}", Type = TableWidthUnitValues.Dxa },
                Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
            }
        };
        paragraph = new Paragraph(new Run(new Text(member.DefaultValue))
        {
            RunProperties = new RunProperties
            {
                NoProof = new NoProof { Val = new OnOffValue(true) },
            }
        })
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
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
                Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
            }
        };

        if (!string.IsNullOrEmpty(member.Descriptions?[culture]))
        {
            if (Helpers.DocumentHelper.MarkdownToParagraph(member.Descriptions![culture]) is IEnumerable<OpenXmlElement> xElements)
            {
                tableCell.Append(xElements);
            }
        }
        else
        {
            tableCell.Append(new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
                    KeepNext = new KeepNext()
                }
            });
        }
        
        tableRow.Append(tableCell);

        return tableRow;
    }

}
