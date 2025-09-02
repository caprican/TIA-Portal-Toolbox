using Microsoft.Extensions.Options;

namespace TiaPortalToolbox.Table.Factories;

public class SheetFactory(IOptions<Models.SpreadsheetSettings> settings) : Contracts.Factories.ISheetFactory
{
    private readonly Models.SpreadsheetSettings? settings = settings?.Value;

    public Contracts.Builders.ISheetBuilder CreateSheet()
    {
        throw new NotImplementedException();
    }
}
