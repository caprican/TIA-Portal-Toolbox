using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Markdig;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;

using static DocumentFormat.OpenXml.Wordprocessing.TableExtensions;

namespace TiaPortalToolbox.Doc.Builders;

public class PlcBlockPageBuilder(IOptions<Models.DocumentSettings> settings, WordprocessingDocument document
                                , Core.Models.ProjectTree.Plc.Blocks.Object plcItem, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems) : IPageBuilder
{
    private readonly Models.DocumentSettings settings = settings.Value;
    private readonly WordprocessingDocument document = document;
    private Markdig.Renderers.Docx.Contracts.IDocumentStyle documentStyle => settings.DocumentStyle;

    internal readonly int TableSize = 8607;

    private readonly int WhiteColumnSize = 500;
    private readonly int VersionColumnSize = 1250;
    private readonly int DescriptionComulnSize = 6857;

    public void Build()
    {
        List<Core.Models.ProjectTree.Plc.Object> functionUserDefines = [];

        document.BodyAppend(new Paragraph(new Run(new Text(plcItem.Name)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = settings.DocumentStyle.Headings[3] }
            }
        });

        if (!string.IsNullOrEmpty(plcItem.Author?[settings.Culture]))
        {
            document.BodyAppend(new Paragraph(new Run(new Text($"{Properties.Resources.AuthorParagraph} : {plcItem.Author![settings.Culture]}"))
            {
                RunProperties = new RunProperties
                {
                    FontSize = new FontSize { Val = "16" }
                }
            })
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockText"] },
                    //ParagraphStyleId = new ParagraphStyleId { Val = "Blocktext" },
                    //SpacingBetweenLines = new SpacingBetweenLines { Before = "0", After = "0" },
                    //Indentation = new Indentation { Left = "1200" },
                    //Justification = new Justification { Val = JustificationValues.Both },
                    //ParagraphMarkRunProperties = new ParagraphMarkRunProperties(new FontSize { Val = "16" })
                }
            });
        }

        if (!string.IsNullOrEmpty(plcItem.Function?[settings.Culture]))
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.ShortDescriptionParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
                }
            });

            document.MarkdownConvert(settings, plcItem.Function![settings.Culture]);
        }

        document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.InterfaceDescriptionParagraph)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
            }
        });
        document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.BlockInterfaceParagraph)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
            }
        });
        var blockDraw = new GraphicBlockBuilder(settings);
        document.BodyAppend(blockDraw.BlockDraw(plcItem.DisplayName ?? plcItem.Name, plcItem.IsSafetyBlock, plcItem.Members?[settings.Culture]));

        var inputMember = plcItem.Members?[settings.Culture].Where(member => member.Direction == Core.Models.DirectionMember.Input);
        if (inputMember.Any())
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.InputParameterParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] },
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                }
            });
            GenerateTableParameter(inputMember!);

            inputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var outputMember = plcItem.Members?[settings.Culture].Where(member => member.Direction == Core.Models.DirectionMember.Output);
        if (outputMember?.Count() > 0)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.OutputParameterParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] },
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                }
            });
            GenerateTableParameter(outputMember!);

            outputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var inoutputMember = plcItem.Members?[settings.Culture].Where(member => member.Direction == Core.Models.DirectionMember.InOutput);
        if (inoutputMember?.Count() > 0)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.InOutParameterParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] },
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                }
            });
            GenerateTableParameter(inoutputMember!);

            inoutputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var staticMember = plcItem.Members?[settings.Culture].Where(member => member.Direction == Core.Models.DirectionMember.Static);
        if (staticMember?.Count() > 0)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.StaticsParameterParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] },
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                }
            });
            GenerateTableParameter(staticMember!);

            staticMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        if (functionUserDefines.Count > 0)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.UdtParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
                }
            });
            var chapterBuilder = new UserDatatypeChapter(settings, document);
            foreach (var userDefineType in functionUserDefines.OfType<Core.Models.ProjectTree.Plc.Type>())
            {
                var paragraph = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] },
                    }
                };
                paragraph.Append(new Run(new Text($"{userDefineType.Name} ("))
                {
                    RunProperties = new RunProperties
                    {
                        NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                    }
                });

                if (userDefineType.IsSafetyBlock)
                {
                    paragraph.Append(new Run(new Text($"SafetyUDT"))
                    {
                        RunProperties = new RunProperties
                        {
                            Shading = new Shading { Color = "auto", Fill = "FFFF33", Val = ShadingPatternValues.Clear },
                            NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                        }
                    });
                }
                else
                {
                    paragraph.Append(new Run(new Text($"UDT"))
                    {
                        RunProperties = new RunProperties
                        {
                            NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                        }
                    });
                }

                if (!string.IsNullOrEmpty(userDefineType.Version))
                {
                    paragraph.Append(new Run(new Text($" / V{userDefineType.Version}") { Space = SpaceProcessingModeValues.Preserve })
                    {
                        RunProperties = new RunProperties
                        {
                            NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                        }
                    });
                }

                paragraph.Append(new Run(new Text(")") { Space = SpaceProcessingModeValues.Preserve })
                {
                    RunProperties = new RunProperties
                    {
                        NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                    }
                });
                document.BodyAppend(paragraph);

                chapterBuilder.Build(userDefineType);
            }
        }

        if (!string.IsNullOrEmpty(plcItem.Descriptions?[settings.Culture]))
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.FonctionalDescriptionParagraph)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
                }
            });
            document.MarkdownConvert(settings, plcItem.Descriptions![settings.Culture]);
        }

        if (plcItem.Logs?[settings.Culture] is not null)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(Properties.Resources.ChangeLogHeader)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = documentStyle.MarkdownStyles["BlockTitle"] }
                }
            });
            GenerateTableLog(plcItem.Logs[settings.Culture]);
        }
    }

    private void GenerateTableParameter(IEnumerable<Core.Models.InterfaceMember> members)
    {
        var _asDefaultValue = members.Any(member => !string.IsNullOrEmpty(member.DefaultValue));

        var markdownTable = $"{Properties.Resources.IdentifierColumn} | {Properties.Resources.DatatypeColumn}{(_asDefaultValue ? $" | {Properties.Resources.DefaultvalueColumn} " : " ")}| {Properties.Resources.DescriptionColumn}\n --- | --- | {(_asDefaultValue ? "-- | " : "")}--- |\n";

        foreach (var member in members)
        {
            if (member.Descriptions?.ContainsKey(settings.Culture) == true)
            {
                markdownTable += $"{member.Name} | {member.Type}{(_asDefaultValue ? $" | {member.DefaultValue} " : " ")} | {member.Descriptions?[settings.Culture] ?? string.Empty} | \n";
            }
            else
            {
                markdownTable += $"{member.Name} | {member.Type}{(_asDefaultValue ? $" | {member.DefaultValue} " : " ")} | | \n";
            }
        }
        markdownTable = markdownTable.RemoveLast(1);

        var renderer = new Markdig.Renderers.Docx.DocxDocumentRenderer(document, settings.DocumentStyle)
        {
            ImagesBaseUri = settings.UserFolderPath,
            LinksBaseUri = "",
            SkipImages = false,
        };
        renderer.ObjectRenderers.RemoveAll(r => r.GetType() == typeof(Markdig.Renderers.Docx.Extensions.TableRenderer));
        renderer.ObjectRenderers.Add(new Renderers.IoTableRenderer());

        var pipeline = new Markdig.MarkdownPipelineBuilder().UseAdvancedExtensions()
                                                    .UseEmojiAndSmiley()
                                                    .Build();

        var markdownDocument = Markdig.Markdown.Parse(markdownTable, pipeline);

        renderer.Render(markdownDocument);
    }

    private void GenerateTableLog(List<Core.Models.PlcBlockLog> logs)
    {
        var tableStyle = settings.DocumentStyle.TableStyles["LogTable"] as Markdig.Renderers.Docx.DocumentStyleTable;
        var columns = new List<int> { WhiteColumnSize, VersionColumnSize, DescriptionComulnSize };

        var table = new Table().CreateTable(columns, styleName:tableStyle!.StyleName, indentation:113 );

        //table.Append( new TableRow(new TableCell(new Paragraph(new Run(new Text("Version & Date")))
        //{
        //    ParagraphProperties = new ParagraphProperties
        //    {
        //        ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
        //    }
        //})
        //{
        //    TableCellProperties = new TableCellProperties
        //    {
        //        TableCellWidth = new TableCellWidth { Width = $"{settings.IdentifierColumnSize}", Type = TableWidthUnitValues.Dxa },
        //        GridSpan = new GridSpan { Val = 2 },
        //        TableCellBorders = new TableCellBorders
        //        {
        //            BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
        //        },
        //        Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
        //        NoWrap = new NoWrap(),
        //        TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        //    }
        //},
        //new TableCell(new Paragraph(new Run(new Text("Change description")))
        //{
        //    ParagraphProperties = new ParagraphProperties
        //    {
        //        ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft }
        //    }
        //})
        //{
        //    TableCellProperties = new TableCellProperties
        //    {
        //        TableCellWidth = new TableCellWidth { Width = $"{settings.DataTypeColumnSize}", Type = TableWidthUnitValues.Dxa },
        //        TableCellBorders = new TableCellBorders
        //        {
        //            BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
        //        },
        //        Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
        //        NoWrap = new NoWrap(),
        //        TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
        //    }
        //})
        //{
        //    TableRowProperties = new TableRowProperties(new CantSplit(), new DocumentFormat.OpenXml.Wordprocessing.TableHeader())
        //});

        table.BuildHeader(
        [
            new HeaderDefine
            {
                Title = Properties.Resources.VersionColumn,
                Width = (uint)settings.IdentifierColumnSize,
                GridSpan = 2,
                StyleName = tableStyle.StyleTextLeft,
                BottomBorder = new BorderStyle
                {
                    Color = tableStyle.BorderColor,
                    Space = tableStyle.BorderSpace,
                    Size = tableStyle.Header!.BorderSize
                },
                ShadingColor = tableStyle.ShadingColor,
                ShadingFill = tableStyle.Header.ShadingFill
            },
            new HeaderDefine
            {
                Title = Properties.Resources.ChangeDescriptionColumn,
                StyleName = tableStyle.StyleTextCenter,
                BottomBorder = new BorderStyle
                {
                    Color = tableStyle.BorderColor,
                    Space = tableStyle.BorderSpace,
                    Size = tableStyle.Header!.BorderSize
                },
                ShadingColor = tableStyle.ShadingColor,
                ShadingFill = tableStyle.Header.ShadingFill
            }
        ]);

        foreach (var log in logs)
        {
            var tableRow = new TableRow
            {
                TableRowProperties = new TableRowProperties(new CantSplit())
            };

            var tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{WhiteColumnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Nil },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = tableStyle!.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Nil }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            var paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{VersionColumnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            paragraph1.Append(new Run(new Text(log.Version ?? string.Empty))
            {
                RunProperties = new RunProperties
                {
                    Bold = new Bold(),
                }
            });
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{DescriptionComulnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            paragraph1.Append(new Run(new Text(log.Author ?? string.Empty))
            {
                RunProperties = new RunProperties
                {
                    Bold = new Bold(),
                    NoProof = new NoProof { Val = new OnOffValue(true) },
                }
            });
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);

            table.Append(tableRow);

            tableRow = new TableRow
            {
                TableRowProperties = new TableRowProperties(new CantSplit())
            };

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{WhiteColumnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Nil },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        BottomBorder = log.Equals(logs.Last()) ? new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor } : new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Nil }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{VersionColumnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Nil },
                        LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            paragraph1.Append(new Run(new Text(log.Edited?.ToShortDateString() ?? string.Empty))
            {
                RunProperties = new RunProperties()
            });
            tableCell.Append(paragraph1);

            tableRow.Append(tableCell);

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{DescriptionComulnSize}", Type = TableWidthUnitValues.Dxa },
                    TableCellBorders = new TableCellBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Nil },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.BorderSize, Color = tableStyle.BorderColor }
                    },
                    Shading = new Shading { Color = tableStyle.BorderColor, Fill = tableStyle.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };

            if (!string.IsNullOrEmpty(log.Description))
            {
                paragraph1 = new Paragraph
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = tableStyle.StyleTextLeft },
                        KeepNext = new KeepNext(),
                        WidowControl = new WidowControl { Val = false },
                        //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                    }
                };
                paragraph1.Append(new Run(new Text(log.Description!))
                {
                    RunProperties = new RunProperties()
                });

                tableCell.Append(paragraph1);
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

            table.Append(tableRow);
        }

        document.BodyAppend(table);
    }
}
