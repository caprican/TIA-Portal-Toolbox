using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Table.Contracts.Builders;

namespace TiaPortalToolbox.Table.Builders;

public class SpreadsheetHmiAlarmsBuilder(IOptions<Models.SpreadsheetSettings> settings) : ISpreadsheetBuilder
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
                        row.Append(new Cell() { CellValue = new CellValue("ID"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Name"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Alarm text [{culture.Name}], Alarm text 1"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 1]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Class"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Trigger tag"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Trigger bit"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Trigger mode"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Acknowledgement tag"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Acknowledgement bit"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("PLC acknowledgement tag"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("PLC acknowledgement bit"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Priority"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Info text [{culture.Name}], Info text"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 1 [{culture.Name}], Alarm text 2"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 2]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 2 [{culture.Name}], Alarm text 3"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 3]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 3 [{culture.Name}], Alarm text 4"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 4]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 4 [{culture.Name}], Alarm text 5"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 5]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 5 [{culture.Name}], Alarm text 6"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 6]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 6 [{culture.Name}], Alarm text 7"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 7]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 7 [{culture.Name}], Alarm text 8"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 8]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 8 [{culture.Name}], Alarm text 9"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 9]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue($"Additional text 9 [{culture.Name}], Alarm text 10"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("FieldInfo [Alarm text 10]"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 1"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 2"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 3"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 4"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 5"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 6"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 7"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 8"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 9"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Alarm parameter 10"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Area"), DataType = CellValues.String });
                        row.Append(new Cell() { CellValue = new CellValue("Origin"), DataType = CellValues.String });
                        
                        sheetData.Append(row);

                        if(connexion.Tags?.Count > 0)
                        {
                            foreach (var tag in connexion.Tags)
                            {
                                var txt = string.Empty;
                                tag.Descriptions?.TryGetValue(culture, out txt);

                                row = new Row();
                                row.Append(new Cell() { CellValue = new CellValue((int)tag.Id), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue(tag.HmiTagname ?? "<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(txt), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(string.Empty), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.ClassAlarm ?? "Alarm"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.HmiTagname ?? string.Empty), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("On rising edge"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("0"), DataType = CellValues.Number });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue("<No value>"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue($"{connexion.HmiName}::Alarming"), DataType = CellValues.String });
                                row.Append(new Cell() { CellValue = new CellValue(tag.FolderName ?? string.Empty), DataType = CellValues.String });

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
