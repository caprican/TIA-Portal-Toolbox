using System.Globalization;

namespace TiaPortalToolbox.Table.Contracts.Builders;

public interface ISpreadsheetBuilder
{
    public Task CreateSpreadsheet(TiaPortalOpenness.Models.ProjectTree.Connexion connexion, CultureInfo culture);
}
