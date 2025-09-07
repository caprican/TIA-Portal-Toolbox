using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Table.Contracts.Builders;

namespace TiaPortalToolbox.Table.Builders;

public class SpreadsheetHmiTagsBuilder(IOptions<Models.SpreadsheetSettings> settings) : ISpreadsheetBuilder
{
    private readonly Models.SpreadsheetSettings? settings = settings?.Value;

    public Task CreateSpreadsheet(TiaPortalOpenness.Models.ProjectTree.Connexion connexion, CultureInfo culture)
    {
        var tcs = new TaskCompletionSource<bool>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if (settings is null)
                {
                    return;
                }

                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(settings.SpreadsheetPath, SpreadsheetDocumentType.Workbook))
                {

                    // Add a WorkbookPart to the document.
                    var workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add Sheets to the Workbook.
                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    // Append a new worksheet and associate it with the workbook.
                    Sheet sheet = new() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "DiscreteAlarms" };
                    sheets.Append(sheet);

                    var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    if(!sheetData.Elements<Row>().Any())
                    {
                        var row = new Row();
                        row.Append(new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Path"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Connection"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("PLC tag"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("DataType"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("HMI DataType"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Length"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Access Method"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Address"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Start value"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Persistency"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Substitute value"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("ID tag"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Comment [{culture.Name}]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Acquisition mode"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Acquisition cycle"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Limit Upper 2 Type"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Limit Lower 2 Type"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Limit Lower 2"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Linear scaling"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("End value PLC"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Start value PLC"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("End value HMI"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Start value HMI"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Gmp relevant"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Confirmation Type"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("RequiredFunctionRights"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Mandatory Commenting"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Scope"), DataType = CellValues.String });
                        
                        sheetData.Append(row);

                        if(connexion.Tags?.Count > 0)
                        {
                            foreach (var tag in connexion.Tags)
                            {
                                var txt = string.Empty;
                                tag.Descriptions?.TryGetValue(culture, out txt);

                                row = new Row();
                                row.Append(new Cell() { CellValue = new CellValue(tag.HmiTagname ?? string.Empty), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.FolderName ?? string.Empty), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(connexion.Name), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.Tagname ?? string.Empty), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.Datatype ?? "<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.Datatype ?? "<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("1"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("Symbolic access"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No Value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("False"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("Cyclic in operation"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("T1s"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("None"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("False"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("10"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("100"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("False"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("None"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("False"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("System-wide"), DataType = CellValues.String });

                                sheetData.Append(row);
                            }
                        }
                    }
                    else
                    {
                        foreach(var row in sheetData.Elements<Row>())
                        {
                            foreach(var cell in row.Elements<Cell>())
                            {
                                //cell.CellValue.Text
                            }
                        }
                    }
                }




                tcs.SetResult(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

}
