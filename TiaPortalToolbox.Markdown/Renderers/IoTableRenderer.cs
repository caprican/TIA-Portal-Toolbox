using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Renderers;

internal class IoTableRenderer : Markdig.Renderers.Docx.DocxObjectRenderer<Markdig.Extensions.Tables.Table>
{
    protected override void WriteObject(Markdig.Renderers.Docx.DocxDocumentRenderer renderer, Markdig.Extensions.Tables.Table obj)
    {
        renderer.ForceCloseParagraph();

        var indentation = 113;
        var tableStyle = renderer.Styles.TableStyles["IoTable"];

        var table = new Table(new TableProperties
        {
            TableStyle = new TableStyle { Val = tableStyle.StyleName },
            //TableWidth = new TableWidth { Width = $"{width}", Type = TableWidthUnitValues.Dxa },
            TableWidth = new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            TableIndentation = new TableIndentation { Width = indentation, Type = TableWidthUnitValues.Dxa },
            TableLook = new TableLook { Val = "04A0", FirstRow = true, LastRow = false, FirstColumn = true, LastColumn = false, NoHorizontalBand = false, NoVerticalBand = true },
        });
        renderer.Cursor.Write(table);

        bool firstRow = true;
        foreach (var row in obj.OfType<Markdig.Extensions.Tables.TableRow>())
        {
            var tableRow = new TableRow();
            if (firstRow)
            {
                tableRow.AddChild(new TableRowProperties(new TableHeader() ));
            }
            table.Append(tableRow);
            foreach (var cell in row.OfType<Markdig.Extensions.Tables.TableCell>())
            {
                var tableCell = new TableCell();
                if (firstRow)
                {
                    tableCell.Append(new TableCellProperties
                    {
                        //TableCellWidth = new TableCellWidth { Width = $"{header.Width}", Type = TableWidthUnitValues.Dxa },
                        //GridSpan = header.GridSpan > 0 ? new GridSpan { Val = header.GridSpan } : null,
                        TableCellBorders = new TableCellBorders
                        {
                            BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = tableStyle.BorderSpace, Size = tableStyle.Header!.BorderSize, Color = tableStyle.BorderColor }
                        },
                        Shading = new Shading { Color = tableStyle.ShadingColor, Fill = tableStyle.Header.ShadingFill, Val = ShadingPatternValues.Clear },
                        NoWrap = new NoWrap(),
                        TableCellVerticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    });
                }
                else
                {
                    tableCell.Append(new TableCellProperties
                    {
                        //TableCellWidth = new TableCellWidth { Width = $"{settings.DefaultValueColumnSize}", Type = TableWidthUnitValues.Dxa },
                        Shading = new Shading { Color = tableStyle.ShadingColor, Fill = "Auto"/*member.Islocked ? settings.ShadingFillLock : settings.ShadingFill*/, Val = ShadingPatternValues.Clear }
                    });
                }
                tableRow.Append(tableCell);
                renderer.Cursor.GoInto(tableCell);
                renderer.WriteChildren(cell);
            }

            firstRow = false;
        }
        renderer.Cursor.SetAfter(table);
    }
}
