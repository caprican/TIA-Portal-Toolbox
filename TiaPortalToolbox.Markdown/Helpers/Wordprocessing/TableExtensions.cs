namespace DocumentFormat.OpenXml.Wordprocessing;

public static class TableExtensions
{
    public static Table CreateTable(this Table table, List<int> columns, string? styleName = null, int? width = null, int? indentation = null)
    {
        table = new Table(new TableProperties
        {
            TableStyle = styleName is null ? null : new TableStyle { Val = styleName },
            TableWidth = width is null ? new TableWidth { Width="5000", Type= TableWidthUnitValues.Pct } :  new TableWidth { Width = $"{width}", Type = TableWidthUnitValues.Dxa },
            TableIndentation = indentation is null ? null : new TableIndentation { Width = indentation, Type = TableWidthUnitValues.Dxa },
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

    public static void BuildHeader (this Table table, HeaderDefine[] headers)
    {
        var tableRow = new TableRow
        {
            TableRowProperties = new TableRowProperties(new CantSplit(), new TableHeader())
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
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = header.BottomBorder!.Space, Size = header.BottomBorder!.Size, Color = header.BottomBorder.Color }
                    },
                    Shading = new Shading { Color = header.ShadingColor, Fill = header.ShadingFill, Val = ShadingPatternValues.Clear },
                    NoWrap = new NoWrap(),
                    TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                }
            };
            var paragraph1 = new Paragraph(new Run(new Text(header.Title ?? string.Empty)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = header.StyleName }
                }
            };

            tableCell.Append(paragraph1);
            tableRow.Append(tableCell);
        }

        table.Append(tableRow);
    }

    public record HeaderDefine
    {
        public string? Title { get; set; } 
        public uint? Width { get; set; } 
        public uint? Indentation { get; set; }
        public int? GridSpan { get; set; }

        public string? StyleName { get; set; }
        public string? ShadingFill { get; set; }
        public string? ShadingColor { get; set; }

        public BorderStyle? BottomBorder { get; set; }
    }

    public record BorderStyle
    {
        public string Color { get; set; } = "Auto";
        public uint Space { get; set; } 
        public uint Size { get; set; }
    }
}
