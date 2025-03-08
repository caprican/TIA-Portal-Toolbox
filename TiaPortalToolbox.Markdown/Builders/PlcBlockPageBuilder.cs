using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;

namespace TiaPortalToolbox.Doc.Builders;

public class PlcBlockPageBuilder(IOptions<Models.DocumentSettings> settings, Core.Models.ProjectTree.Plc.Blocks.Object plcItem, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems) : IPageBuilder
{
    private readonly Models.DocumentSettings settings = settings.Value;

    private int DescriptionColumnSize;// = 5771;

    internal readonly int TableSize = 8607;

    private readonly int WhiteColumnSize = 500;
    private readonly int VersionColumnSize = 1250;
    private readonly int DescriptionComulnSize = 6857;

    public OpenXmlElement[] Build(CultureInfo culture)
    {
        culture ??= CultureInfo.InvariantCulture;
        List<Core.Models.ProjectTree.Plc.Object> functionUserDefines = [];
        var body = new List<OpenXmlElement>();

        body.Add(new Paragraph(new Run(new Text(plcItem.Name)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading3].Name }
            }
        });

        if (!string.IsNullOrEmpty(plcItem.Author?[culture]))
        {
            body.Add(new Paragraph(new Run(new Text($"Author : {plcItem.Author![culture]}"))
            {
                RunProperties = new RunProperties
                {
                    FontSize = new FontSize { Val = "16" }
                }
            }) 
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockText].Name },
                    //ParagraphStyleId = new ParagraphStyleId { Val = "Blocktext" },
                    //SpacingBetweenLines = new SpacingBetweenLines { Before = "0", After = "0" },
                    //Indentation = new Indentation { Left = "1200" },
                    //Justification = new Justification { Val = JustificationValues.Both },
                    //ParagraphMarkRunProperties = new ParagraphMarkRunProperties(new FontSize { Val = "16" })
                }
            });
        }

        if (!string.IsNullOrEmpty(plcItem.Function?[culture]))
        {
            body.Add(new Paragraph(new Run(new Text("Short description")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            if(Helpers.DocumentHelper.MarkdownToParagraph(plcItem.Function![culture]) is IEnumerable<OpenXmlElement> xElements)
            {
                body.AddRange(xElements);
            }
        }

        body.Add(new Paragraph(new Run(new Text("Interface description")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
            }
        });
        body.Add(new Paragraph(new Run(new Text("Block interface")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
            }
        });
        var blockDraw = new GraphicBlockBuilder(settings);
        body.Add(blockDraw.BlockDraw(plcItem.DisplayName ?? plcItem.Name, plcItem.IsSafetyBlock, plcItem.Members?[culture]));

        var inputMember = plcItem.Members?[culture].Where(member => member.Direction == Core.Models.DirectionMember.Input);
        if (inputMember.Any())
        {
            body.Add(new Paragraph(new Run(new Text("Input parameter")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            body.Add(GenerateTableParameter(inputMember!, culture));

            inputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var outputMember = plcItem.Members?[culture].Where(member => member.Direction == Core.Models.DirectionMember.Output);
        if (outputMember.Any())
        {
            body.Add(new Paragraph(new Run(new Text("Output parameter")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            body.Add(GenerateTableParameter(outputMember!, culture));

            outputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var inoutputMember = plcItem.Members?[culture].Where(member => member.Direction == Core.Models.DirectionMember.InOutput);
        if (inoutputMember.Any())
        {
            body.Add(new Paragraph(new Run(new Text("In/Out parameter")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            body.Add(GenerateTableParameter(inoutputMember!, culture));

            inoutputMember.ToList().ForEach(member =>
            {
                if (derivedItems.Any(udt => udt.Name == member.DerivedType))
                {
                    functionUserDefines.Add(derivedItems.First(udt => udt.Name == member.DerivedType));
                }
            });
        }

        var staticMember = plcItem.Members?[culture].Where(member => member.Direction == Core.Models.DirectionMember.Static);
        if (staticMember.Any())
        {
            body.Add(new Paragraph(new Run(new Text("Statics parameter")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            body.Add(GenerateTableParameter(staticMember!, culture));

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
            body.Add(new Paragraph(new Run(new Text("User defined datatype(s)")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });

            body.AddRange(AddUserDefineType(functionUserDefines.OfType<Core.Models.ProjectTree.Plc.Type>(), culture));
        }

        if (!string.IsNullOrEmpty(plcItem.Descriptions?[culture]))
        {
            body.Add(new Paragraph(new Run(new Text("Functional description")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            if (Helpers.DocumentHelper.MarkdownToParagraph(plcItem.Descriptions![culture]) is IEnumerable<OpenXmlElement> xElements)
            {
                body.AddRange(xElements);
            }
        }

        if (plcItem.Logs?[culture] is not null)
        {
            body.Add(new Paragraph(new Run(new Text("Change log")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            body.Add(GenerateTableLog(plcItem.Logs[culture]));
        }

        return [.. body];
    }

    private Table GenerateTableParameter(IEnumerable<Core.Models.InterfaceMember> members, CultureInfo culture)
    {
        var _asDefaultValue = members.Any(member => !string.IsNullOrEmpty(member.DefaultValue));

        DescriptionColumnSize = TableSize - settings.IdentifierColumnSize - settings.DataTypeColumnSize - (_asDefaultValue ? settings.DefaultValueColumnSize : 0);

        var columns = new List<int> { settings.IdentifierColumnSize, settings.DataTypeColumnSize };
        var headerColumns = new List<Models.TableHeader>
            {
                new("Identifier", settings.IdentifierColumnSize),
                new("Data type", settings.DataTypeColumnSize)
            };
        if (_asDefaultValue)
        {
            columns.Add(settings.DefaultValueColumnSize);
            headerColumns.Add(new Models.TableHeader("Default value", settings.DefaultValueColumnSize));
        }
        columns.Add(DescriptionColumnSize);
        headerColumns.Add(new Models.TableHeader("Description", DescriptionColumnSize));

        var table = Helpers.DocumentHelper.CreateTable(TableSize, 113, columns);
        table.Append(Helpers.DocumentHelper.CreateHeaderTable(headerColumns, settings));

        foreach (var member in members)
        {
            var tableRow = new TableRow
            {
                TableRowProperties = new TableRowProperties(new CantSplit())
            };

            var tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{settings.IdentifierColumnSize}", Type = TableWidthUnitValues.Dxa },
                    Shading = new Shading { Color = settings.ShadingColor, Fill = member.Islocked ? settings.ShadingFillLock : settings.ShadingFill, Val = ShadingPatternValues.Clear }
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
                    //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            tableCell.Append(paragraph);
            tableRow.Append(tableCell);

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{settings.DataTypeColumnSize}", Type = TableWidthUnitValues.Auto },
                    Shading = new Shading { Color = settings.ShadingColor, Fill = member.Islocked ? settings.ShadingFillLock : settings.ShadingFill, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph = new Paragraph(new Run(new Text(member.Type!))
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
                    //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            tableCell.Append(paragraph);
            tableRow.Append(tableCell);

            if (_asDefaultValue)
            {
                tableCell = new TableCell
                {
                    TableCellProperties = new TableCellProperties
                    {
                        TableCellWidth = new TableCellWidth { Width = $"{settings.DefaultValueColumnSize}", Type = TableWidthUnitValues.Dxa },
                        Shading = new Shading { Color = settings.ShadingColor, Fill = member.Islocked ? settings.ShadingFillLock : settings.ShadingFill, Val = ShadingPatternValues.Clear }
                    }
                };
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
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextCenter].Name },
                        //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                    }
                };
                tableCell.Append(paragraph);
                tableRow.Append(tableCell);
            }

            tableCell = new TableCell
            {
                TableCellProperties = new TableCellProperties
                {
                    TableCellWidth = new TableCellWidth { Width = $"{DescriptionColumnSize}", Type = TableWidthUnitValues.Dxa },
                    Shading = new Shading { Color = settings.ShadingColor, Fill = member.Islocked ? settings.ShadingFillLock : settings.ShadingFill, Val = ShadingPatternValues.Clear }
                }
            };
            //paragraph = new Paragraph
            //{
            //    ParagraphProperties = new ParagraphProperties
            //    {
            //        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
            //        //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
            //    }
            //};

            //if (!string.IsNullOrEmpty(member.Descriptions?[culture]))
            //{
            //    paragraph.Append(new Run(new Text(member.Descriptions![culture])));
            //}

            //tableCell.Append(paragraph);
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

            table.Append(tableRow);
        }

        return table;
    }

    private Table GenerateTableLog(List<Core.Models.PlcBlockLog> logs)
    {
        var columns = new List<int> { WhiteColumnSize, VersionColumnSize, DescriptionComulnSize };
        var headerColumns = new List<Models.TableHeader>
            {
                new("Version & Date", settings.IdentifierColumnSize, 2),
                new("Change description", settings.DataTypeColumnSize)
            };

        var table = Helpers.DocumentHelper.CreateTable(TableSize, 113, columns);
        table.Append(Helpers.DocumentHelper.CreateHeaderTable(headerColumns, settings));

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
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Nil }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
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
                        TopBorder = new TopBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
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
                        TopBorder = new TopBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
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
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        BottomBorder = log.Equals(logs.Last()) ? new BottomBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor } : new BottomBorder { Val = BorderValues.Nil },
                        RightBorder = new RightBorder { Val = BorderValues.Nil }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
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
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            paragraph1 = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
                }
            };
            paragraph1.Append(new Run(new Text(log.Edited?.ToShortDateString() ?? string.Empty))
            {
                RunProperties = new RunProperties ()
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
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = settings.BorderSpace, Size = settings.BorderSize, Color = settings.BorderColor }
                    },
                    Shading = new Shading { Color = settings.BorderColor, Fill = settings.BorderColor, Val = ShadingPatternValues.Clear }
                }
            };
            //paragraph1 = new Paragraph
            //{
            //    ParagraphProperties = new ParagraphProperties
            //    {
            //        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.TableTextLeft].Name },
            //        KeepNext = new KeepNext(),
            //        WidowControl = new WidowControl { Val = false },
            //        //ParagraphMarkRunProperties = new ParagraphMarkRunProperties()
            //    }
            //};
            //paragraph1.Append(new Run(new Text(log.Description))
            //{
            //    RunProperties = new RunProperties()
            //});

            //tableCell.Append(paragraph1);

            if (!string.IsNullOrEmpty(log.Description))
            {
                if (Helpers.DocumentHelper.MarkdownToParagraph(log.Description!) is IEnumerable<OpenXmlElement> xElements)
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

            table.Append(tableRow);
        }
        return table;
    }

    private IEnumerable<OpenXmlElement> AddUserDefineType(IEnumerable<Core.Models.ProjectTree.Plc.Type> userDefineTypes, CultureInfo culture)
    {
        var xmlElements = new List<OpenXmlElement>();
        foreach (var userDefineType in userDefineTypes)
        {
            var paragraph = new Paragraph
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name },
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
            xmlElements.Add(paragraph);

            if (!string.IsNullOrEmpty(userDefineType.Descriptions?[culture]))
            {
                xmlElements.Add(new Paragraph(new Run(new Text("Description")))
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                    }
                });
                if (Helpers.DocumentHelper.MarkdownToParagraph(userDefineType.Descriptions![culture]) is IEnumerable<OpenXmlElement> xElements)
                {
                    xmlElements.AddRange(xElements);
                }
            }

            xmlElements.Add(new Builders.UserDatatypeChapter(settings).Build(userDefineType, culture));
        }
        return xmlElements;
    }
}
