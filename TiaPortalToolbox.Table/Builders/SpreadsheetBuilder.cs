using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Table.Contracts.Builders;

namespace TiaPortalToolbox.Table.Builders;

public class SpreadsheetBuilder(IOptions<Models.SpreadsheetSettings> settings) : ISpreadsheetBuilder
{
    private readonly Models.SpreadsheetSettings? settings = settings?.Value;

    public Task CreateSpreadsheet()
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
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add Sheets to the Workbook.
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    // Append a new worksheet and associate it with the workbook.
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
                    sheets.Append(sheet);
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
