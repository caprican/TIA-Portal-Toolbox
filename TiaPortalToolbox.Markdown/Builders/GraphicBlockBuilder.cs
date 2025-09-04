using DocumentFormat.OpenXml.Wordprocessing;

using TiaPortalOpenness.Models;

namespace TiaPortalToolbox.Doc.Builders;

internal class GraphicBlockBuilder(Models.DocumentSettings documentSettings)
{
    private readonly Models.DocumentSettings settings = documentSettings;

    private uint functionBlockCenterSize;
    private Models.GraphicBlocStyle? blockStyle;

    internal Table BlockDraw(string blockName, bool isSafetyBlock, List<InterfaceMember>? interfaceMember = null)
    {
        var documentStyle = settings.DocumentStyle as Models.DocumentStyles ?? throw new InvalidOperationException("Document style is not set.");
        blockStyle = documentStyle.TableStyles["Block"] as Models.GraphicBlocStyle ?? throw new InvalidOperationException("Block style is not set.");

        functionBlockCenterSize = blockStyle.TableSize - (2 * (blockStyle.FunctionBlockNameSize + blockStyle.FunctionBlockConnectorSize + blockStyle.FunctionBlockTypeSize));
        var table = new Table();
        table.Append(new TableProperties
        {
            TableStyle = new TableStyle { Val = blockStyle.StyleName },
            TableWidth = new TableWidth { Width = $"{blockStyle.TableSize}", Type = TableWidthUnitValues.Dxa },
            TableIndentation = new TableIndentation { Width = 400, Type = TableWidthUnitValues.Dxa },
            TableBorders = new TableBorders
            {
                TopBorder = new TopBorder { Val = BorderValues.Nil },
                LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                RightBorder = new RightBorder { Val = BorderValues.Nil },
                InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Nil },
                InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Nil }
            },
            TableLook = new TableLook { Val = "05A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = true, NoHorizontalBand = false, NoVerticalBand = true }
        });

        var tableGrid = new TableGrid();
        table.AppendChild(tableGrid);
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockNameSize}" });
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockConnectorSize}" });
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockTypeSize}" });
        tableGrid.Append(new GridColumn { Width = $"{functionBlockCenterSize}" });
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockTypeSize}" });
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockConnectorSize}" });
        tableGrid.Append(new GridColumn { Width = $"{blockStyle.FunctionBlockNameSize}" });

        table.Append(AddTableFunctionHeader(blockName, isSafetyBlock));

        if (interfaceMember is not null)
        {
            table.Append(AddTableFunctionRow(interfaceMember));
        }

        table.Append(AddTableFunctionFooter());

        return table;
    }

    private TableRow AddTableFunctionHeader(string blockName, bool IsSafetyBlock)
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties()
        };

        var tableCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle!.FunctionBlockNameSize + blockStyle.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 2 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                }
            }
        };
        tableCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        tableRow.Append(tableCell);

        tableCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle.TableSize - (2 * blockStyle.FunctionBlockNameSize)}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 3 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header!.BorderSize, Color = blockStyle.BorderColor },
                    LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor },
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor },
                    RightBorder = new RightBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor }
                },
                Shading = new Shading { Color = blockStyle.ShadingColor, Fill = IsSafetyBlock ? blockStyle.ShadingFillSafety : blockStyle.Header!.ShadingFill, Val = ShadingPatternValues.Clear },
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        };
        var paragraph = new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext(),
                Justification = new Justification { Val = JustificationValues.Center },
                WidowControl = new WidowControl { Val = false },
                Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 284 },
                                    new TabStop { Val = TabStopValues.Left, Position = 567 },
                                    new TabStop { Val = TabStopValues.Left, Position = 851 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1134 }),
                SpacingBetweenLines = new SpacingBetweenLines { Before = "60", After = "60" },
                Indentation = new Indentation { Left = "0" }
            }
        };
        paragraph.Append(new Run(new Text(blockName))
        {
            RunProperties = new RunProperties
            {
                RunFonts = new RunFonts { Ascii = "Consolas" },
                FontSize = new FontSize { Val = "20" },
                NoProof = new NoProof(),
            }
        });
        tableCell.Append(paragraph);

        tableRow.Append(tableCell);

        tableCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle.FunctionBlockNameSize + blockStyle.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 2 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                }
            }
        };
        tableCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepNext = new KeepNext(),
                KeepLines = new KeepLines()
            }
        });

        tableRow.Append(tableCell);

        return tableRow;
    }

    private TableRow AddTableFunctionFooter()
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties()
        };

        var tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle!.FunctionBlockNameSize + blockStyle.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 2 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                }
            }
        };
        tableCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        tableRow.Append(tableCell);

        tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle.TableSize - (2 * blockStyle.FunctionBlockNameSize)}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 3 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header!.BorderSize, Color = blockStyle.BorderColor },
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor },
                    RightBorder = new RightBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor }
                },
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        };
        tableCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepNext = new KeepNext(),
                KeepLines = new KeepLines()
            }
        });

        tableRow.Append(tableCell);

        tableCell = new TableCell
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle.FunctionBlockNameSize + blockStyle.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                GridSpan = new GridSpan { Val = 2 },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                }
            }
        };
        tableCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        tableRow.Append(tableCell);

        return tableRow;
    }

    private TableRow[] AddTableFunctionRow(List<InterfaceMember> members)
    {
        var rows = new List<TableRow>();
        var inputsMembers = members?.Where(member => member.Direction == DirectionMember.Input).ToList();
        var outputsMembers = members?.Where(member => member.Direction == DirectionMember.Output).OrderBy(o => !o.Islocked).ToList();

        var membersCount = inputsMembers is null && outputsMembers is null ? 0 : Math.Max(inputsMembers?.Count ?? 0, outputsMembers?.Count ?? 0);
        var membersStart = inputsMembers is null && outputsMembers is null ? 0 : Math.Min(inputsMembers?.Count ?? 0, outputsMembers?.Count ?? 0);
        InterfaceMember memberEmpty = new ()
        {
            Direction = DirectionMember.Other
        };

        if (inputsMembers?.Count == membersStart)
        {
            inputsMembers.AddRange(Enumerable.Repeat(memberEmpty, membersCount - membersStart));
        }
        if (outputsMembers?.Count == membersStart)
        {
            outputsMembers.AddRange(Enumerable.Repeat(memberEmpty, membersCount - membersStart));
        }

        inputsMembers?.AddRange(members?.Where(member => member.Direction == DirectionMember.InOutput)!);
        outputsMembers?.AddRange(members?.Where(member => member.Direction == DirectionMember.InOutput)!);

        for (var i = 0; i < inputsMembers?.Count; i++)
        {
            var topRow = new TableRow
            {
                TableRowProperties = new TableRowProperties()
            };
            var bottomRow = new TableRow
            {
                TableRowProperties = new TableRowProperties()
            };

            var cells = MnemonicName(inputsMembers[i], DirectionMember.Input);
            topRow.Append(cells.TopCell);
            bottomRow.Append(cells.BottomCell);

            cells = Connector(inputsMembers[i], DirectionMember.Input);
            topRow.Append(cells.TopCell);
            bottomRow.Append(cells.BottomCell);

            if (inputsMembers[i]?.Direction != DirectionMember.InOutput)
            {
                cells = MnemonicType(inputsMembers[i], DirectionMember.Input);
                topRow.Append(cells.TopCell);
                bottomRow.Append(cells.BottomCell);
            }

            cells = Center(inputsMembers[i] == outputsMembers![i] ? inputsMembers[i] : memberEmpty);
            topRow.Append(cells.TopCell);
            bottomRow.Append(cells.BottomCell);

            if (outputsMembers[i]?.Direction != DirectionMember.InOutput)
            {
                cells = MnemonicType(outputsMembers[i], DirectionMember.Output);
                topRow.Append(cells.TopCell);
                bottomRow.Append(cells.BottomCell);
            }

            cells = Connector(outputsMembers[i], DirectionMember.Output);
            topRow.Append(cells.TopCell);
            bottomRow.Append(cells.BottomCell);

            cells = MnemonicName(outputsMembers[i], DirectionMember.Output);
            topRow.Append(cells.TopCell);
            bottomRow.Append(cells.BottomCell);
            rows.Add(topRow);
            rows.Add(bottomRow);
        }
        return [.. rows];
    }

    private (TableCell TopCell, TableCell BottomCell) MnemonicName(InterfaceMember member, DirectionMember position)
    {
        var shading = member?.Islocked == true ? blockStyle!.ShadingFillLock : blockStyle!.ShadingFill;
        //var styleId = position == Core.Models.DirectionMember.Input ? "TabelleTextrechts" : "TabelleTextlinks";

        var direction = position switch
        {
            DirectionMember.Input => JustificationValues.Right,
            DirectionMember.Output => JustificationValues.Left,
            _ => JustificationValues.Center
        };

        return Mnemonic($"{blockStyle.FunctionBlockNameSize}", shading, direction, member?.Name ?? string.Empty, member?.HidenInterface == true);
    }

    private (TableCell TopCell, TableCell BottomCell) MnemonicType(InterfaceMember member, DirectionMember position)
    {
        var shading = member?.Islocked == true ? blockStyle!.ShadingFillLock : blockStyle!.ShadingFill;
        //var styleId = position == Core.Models.DirectionMember.Output ? "TabelleTextrechts" : "TabelleTextlinks";

        var direction = position switch
        {
            DirectionMember.Output => JustificationValues.Right,
            DirectionMember.Input => JustificationValues.Left,
            _ => JustificationValues.Center
        };

        return Mnemonic($"{blockStyle.FunctionBlockTypeSize}", shading, direction, member?.Direction != DirectionMember.InOutput ? member?.Type ?? string.Empty : string.Empty , member?.HidenInterface == true);
    }

    private (TableCell TopCell, TableCell BottomCell) Connector(InterfaceMember member, DirectionMember position)
    {
        BottomBorder bottomBorder;
        var topLeftBorder = new LeftBorder { Val = BorderValues.Nil };
        var bottomLeftBorder = new LeftBorder { Val = BorderValues.Nil };
        var topRightBorder = new RightBorder { Val = BorderValues.Nil };
        var bottomRightBorder = new RightBorder { Val = BorderValues.Nil };
        string shading;

        var color = member?.HidenInterface == true ? blockStyle!.ColorHidden : blockStyle!.BorderColor;

        if (member?.Direction == DirectionMember.Other)
        {
            bottomBorder = new BottomBorder { Val = BorderValues.Nil };
            shading = blockStyle.ShadingFill;
        }
        else
        {
            bottomBorder = new BottomBorder { Val = member?.Direction != DirectionMember.InOutput ? BorderValues.Single : BorderValues.Dashed, Space = blockStyle.BorderSpace, Size = blockStyle.BorderSize, Color = color };
            shading = member?.Islocked == true ? blockStyle.ShadingFillLock : blockStyle.ShadingFill;
        }

        switch (position)
        {
            case DirectionMember.Input:
                topRightBorder = new RightBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header!.BorderSize, Color = blockStyle.BorderColor };
                bottomRightBorder = new RightBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor };
                break;
            case DirectionMember.Output:
                topLeftBorder = new LeftBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header!.BorderSize, Color = blockStyle.BorderColor };
                bottomLeftBorder = new LeftBorder { Val = BorderValues.Single, Space = blockStyle.BorderSpace, Size = blockStyle.Header.BorderSize, Color = blockStyle.BorderColor };
                break;
        }

        var topCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle!.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = topLeftBorder,
                    BottomBorder = bottomBorder,
                    RightBorder = topRightBorder
                },
                Shading = new Shading { Color = blockStyle.ShadingColor, Fill = shading, Val = ShadingPatternValues.Clear }
            }
        };
        topCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphMarkRunProperties = new ParagraphMarkRunProperties(new RunFonts { Ascii = "Consolas" }, new FontSize { Val = "14" }),
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        var bottomCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = $"{blockStyle.FunctionBlockConnectorSize}", Type = TableWidthUnitValues.Dxa },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = bottomLeftBorder,
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = bottomRightBorder
                },
                Shading = new Shading { Color = blockStyle.ShadingColor, Fill = shading, Val = ShadingPatternValues.Clear }
            }
        };
        bottomCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphMarkRunProperties = new ParagraphMarkRunProperties(new RunFonts { Ascii = "Consolas" }, new FontSize { Val = "14" }),
                KeepNext = new KeepNext(),
                KeepLines = new KeepLines()
            }
        });

        return (topCell, bottomCell);
    }

    private (TableCell TopCell, TableCell BottomCell) Center(InterfaceMember member)
    {
        var isIO = member?.Direction == DirectionMember.InOutput;
        var cellWidth = $"{functionBlockCenterSize + (Convert.ToInt16(isIO) * (2 * blockStyle!.FunctionBlockTypeSize))}";
        var topCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = cellWidth, Type = TableWidthUnitValues.Dxa },
                GridSpan = member?.Direction == DirectionMember.InOutput ? new GridSpan { Val = 3 } : null,
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = !isIO ? new BottomBorder { Val = BorderValues.Nil } : new BottomBorder { Val = BorderValues.Dashed, Space = blockStyle.BorderSpace, Size = blockStyle.BorderSize, Color = blockStyle.BorderColor },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                },
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Bottom }
            }
        };
        var paragraph = new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                //ParagraphStyleId = new ParagraphStyleId { Val = "TabelleTextzentriert" },
                KeepNext = new KeepNext(),
                KeepLines = new KeepLines(),
                Justification = new Justification { Val = JustificationValues.Center },
                Tabs = new Tabs(new TabStop { Val = TabStopValues.Clear, Position = 284 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 567 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 851 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 1134 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 1418 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 1701 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 1985 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 2268 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 2552 },
                                    new TabStop { Val = TabStopValues.Clear, Position = 2835 }),
            }
        };
        if (!isIO)
        {
            paragraph.Append(new Run
            {
                RunProperties = new RunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Consolas" },
                    FontSize = new FontSize { Val = "14" },
                    NoProof = new NoProof()
                }
            });
        }
        else
        {
            paragraph.Append(new Run(new Text(member?.Type ?? string.Empty))
            {
                RunProperties = new RunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Consolas" },
                    FontSize = new FontSize { Val = "16" },
                    NoProof = new NoProof()
                }
            });
        }
        topCell.Append(paragraph);

        var bottomCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = cellWidth, Type = TableWidthUnitValues.Dxa },
                GridSpan = member?.Direction == DirectionMember.InOutput ? new GridSpan { Val = 3 } : null,
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                }
            }
        };
        bottomCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphMarkRunProperties = new ParagraphMarkRunProperties(new RunFonts { Ascii = "Consolas" }, new FontSize { Val = "14" }),
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        return (topCell, bottomCell);
    }

    private (TableCell TopCell, TableCell BottomCell) Mnemonic(string size, string shading, JustificationValues direction, string text, bool hidden)
    {
        var topCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = size, Type = TableWidthUnitValues.Dxa },
                VerticalMerge = new VerticalMerge { Val = MergedCellValues.Restart },
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                },
                Shading = new Shading { Color = blockStyle!.ShadingColor, Fill = shading, Val = ShadingPatternValues.Clear },
                TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            }
        };

        var paragraph = new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                //ParagraphStyleId = new ParagraphStyleId { Val = styleId },
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext(),
                Justification = new Justification { Val = direction },
                SpacingBetweenLines = new SpacingBetweenLines { Before = "40", After = "40" },
                Indentation = new Indentation { Left = "0" },
            }
        };

        if (!string.IsNullOrEmpty(text))
        {
            paragraph.Append(new Run(new Text(text))
            {
                RunProperties = new RunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Consolas" },
                    FontSize = new FontSize { Val = "16" },
                    NoProof = new NoProof(),
                    Color = new Color { Val = hidden ? blockStyle.ColorHidden : "Auto" }
                }
            });
        }

        topCell.Append(paragraph);

        var bottomCell = new TableCell()
        {
            TableCellProperties = new TableCellProperties
            {
                TableCellWidth = new TableCellWidth { Width = size, Type = TableWidthUnitValues.Dxa },
                VerticalMerge = new VerticalMerge(),
                TableCellBorders = new TableCellBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                    RightBorder = new RightBorder { Val = BorderValues.Nil }
                },
                Shading = new Shading { Color = blockStyle.ShadingColor, Fill = shading, Val = ShadingPatternValues.Clear },
            }
        };
        bottomCell.Append(new Paragraph
        {
            ParagraphProperties = new ParagraphProperties
            {
                KeepLines = new KeepLines(),
                KeepNext = new KeepNext()
            }
        });

        return (topCell, bottomCell);
    }
}
