using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Builders;

internal class UserDatatypeChapter(Models.DocumentSettings settings, WordprocessingDocument document)
{
    private readonly Models.DocumentSettings settings = settings;

    internal readonly int TableSize = 8607;
    private int IdentifierColumnSize = 1418;
    private int DescriptionColumnSize;

    public void Build(Core.Models.ProjectTree.Plc.Type userDefineType)
    {
        if (!string.IsNullOrEmpty(userDefineType.Descriptions?[settings.Culture]))
        {
            //document.BodyAppend(new Paragraph(new Run(new Text("Description")))
            //{
            //    ParagraphProperties = new ParagraphProperties
            //    {
            //        ParagraphStyleId = new ParagraphStyleId { Val = settings.DocumentStyle.MarkdownStyles["BlockTitle"] }
            //    }
            //});
            document.MainDocumentPart?.AddParagraph("Description", settings.DocumentStyle.MarkdownStyles["BlockTitle"]);
            document.MarkdownConvert(settings, userDefineType.Descriptions![settings.Culture]);
        }

        //document.BodyAppend(new Paragraph(new Run(new Text("Parameter description")))
        //{
        //    ParagraphProperties = new ParagraphProperties
        //    {
        //        ParagraphStyleId = new ParagraphStyleId { Val = settings.DocumentStyle.MarkdownStyles["BlockTitle"] }
        //    }
        //});
        document.MainDocumentPart?.AddParagraph("Parameter description", settings.DocumentStyle.MarkdownStyles["BlockTitle"]);
        BuildTable(userDefineType);
    }

    private void BuildTable(Core.Models.ProjectTree.Plc.Type userDefineType)
    {
        var levelMax = GetLevelCount(userDefineType.Members?[settings.Culture]);

        IdentifierColumnSize += (levelMax * settings.IdentColumnSize / 10);
        //DescriptionColumnSize = settings.TableSize - IdentifierColumnSize - settings.DataTypeColumnSize - settings.DefaultValueColumnSize;

        var columns = Enumerable.Repeat(settings.IdentColumnSize, levelMax + 1).ToList();
        columns.Add(settings.DataTypeColumnSize);
        columns.Add(settings.DefaultValueColumnSize);
        columns.Add(DescriptionColumnSize);

        var tableStyle = settings.DocumentStyle.TableStyles["Default"] as Markdig.Renderers.Docx.DocumentStyleTable;
        var table = new Table().CreateTable(columns, tableStyle!.StyleName, indentation: 113);
        //table.Append(Helpers.DocumentHelper.CreateHeaderTable(
        //    [
        //        new("Identifier", IdentifierColumnSize, levelMax + 1),
        //        new("Data type", settings.DataTypeColumnSize),
        //        new("Default value", settings.DefaultValueColumnSize),
        //        new("Description", DescriptionColumnSize)
        //    ], settings));

        table.Append(new TableRow(new TableCell(new Paragraph(new Run(new Text("Identifier")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
            }
        })
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{IdentifierColumnSize}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = levelMax + 1 },
                TableCellBorders = new TableCellBorders
                {
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle!.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
                },
                Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        },
        new TableCell(new Paragraph(new Run(new Text("Data type")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
            }
        })
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{settings.DataTypeColumnSize}", Type = TableWidthUnitValues.Dxa },
                TableCellBorders = new TableCellBorders
                {
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
                },
                Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        },
        new TableCell(new Paragraph(new Run(new Text("Default value")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
            }
        })
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{settings.DefaultValueColumnSize}", Type = TableWidthUnitValues.Dxa },
                TableCellBorders = new TableCellBorders
                {
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
                },
                Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        },
        new TableCell(new Paragraph(new Run(new Text("Description")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
            }
        })
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{DescriptionColumnSize}", Type = TableWidthUnitValues.Dxa },
                TableCellBorders = new TableCellBorders
                {
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
                },
                Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
                NoWrap = new NoWrap(),
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        })
        {
            TableRowProperties = new TableRowProperties(new CantSplit(), new DocumentFormat.OpenXml.Wordprocessing.TableHeader())
        });

        if (userDefineType.Members?[settings.Culture] is not null)
        {
            table.Append(BuildCorp(userDefineType.Members[settings.Culture], 0, levelMax));
        }

        document.BodyAppend(table);
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

    private TableRow[] BuildCorp(List<Core.Models.InterfaceMember> members, int level, int levelMax)
    {
        var rows = new List<TableRow>();
        foreach (var member in members)
        {
            rows.Add(AddRow(member, settings.Culture, level, levelMax, member == members.Last()));
            if (member.Members?.Count() > 0)
            {
                rows.AddRange(BuildCorp(member.Members, level + 1, levelMax));
            }
        }

        return [.. rows];
    }

    private TableRow AddRow(Core.Models.InterfaceMember member, CultureInfo culture, int level, int levelMax, bool isLastRow)
    {
        var tableStyle = settings.DocumentStyle.TableStyles["Default"] as Markdig.Renderers.Docx.DocumentStyleTable;

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
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear },
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
                TableCellWidth = new TableCellWidth { Width = $"{settings.DataTypeColumnSize}", Type = TableWidthUnitValues.Dxa },
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
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
                TableCellWidth = new TableCellWidth { Width = $"{settings.DefaultValueColumnSize}", Type = TableWidthUnitValues.Dxa },
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
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
                //Shading = new Shading { Color = settings.ShadingColor, Fill = settings.ShadingFill, Val = ShadingPatternValues.Clear }
            }
        };

        if (member.Descriptions?.ContainsKey(culture) == true && !string.IsNullOrEmpty(member.Descriptions?[culture]))
        {
            tableCell.Append(new Paragraph(new Run(new Text(member.Descriptions?[culture] ?? string.Empty))
            {
                RunProperties = new RunProperties
                {
                    NoProof = new NoProof { Val = new OnOffValue(true) },
                }
            })
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                    KeepNext = new KeepNext()
                }
            });
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
