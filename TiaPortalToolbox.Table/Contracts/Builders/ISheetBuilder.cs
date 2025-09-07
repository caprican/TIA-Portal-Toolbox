using DocumentFormat.OpenXml.Packaging;

namespace TiaPortalToolbox.Table.Contracts.Builders;

public interface ISheetBuilder
{
    internal void Build(WorksheetPart worksheetPart);
}
