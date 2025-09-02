using System.Globalization;

namespace TiaPortalToolbox.Table.Models;

public class SpreadsheetSettings
{
    public string ProjectPath { get; set; } = string.Empty;

    public string UserFolderPath { get; set; } = string.Empty;
    public string SpreadsheetPath { get; set; } = string.Empty;

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
}
