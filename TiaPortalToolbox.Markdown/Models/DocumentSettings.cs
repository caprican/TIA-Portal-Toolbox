namespace TiaPortalToolbox.Doc.Models;

public class DocumentSettings
{
    internal readonly string BorderColor = "Auto";
    internal readonly uint BorderSpace = 0;
    internal readonly uint BorderSize = 4;
    internal readonly uint BorderHeaderSize = 12;

    internal readonly string ShadingColor = "Auto";
    internal readonly string ShadingFill = "Auto";
    internal readonly string ShadingFillLock = "EEEEEE";
    internal readonly string ShadingFillHeader = "E6E6E6";
    internal readonly string ShadingFillNote = "FFFFFF";
    internal readonly string ShadingFillSafety = "FFFF33";

    internal readonly uint FunctionBlockNameSize = 2400;
    internal readonly uint FunctionBlockConnectorSize = 250;
    internal uint FunctionBlockCenterSize;
    internal readonly uint FunctionBlockTypeSize = 1300;

    internal readonly int TableSize = /*8508*/8000;
    internal readonly string ColorHidden = "777777";

    internal readonly int IdentColumnSize = 220;
    internal readonly int DataTypeColumnSize = 1418;
    internal readonly int DefaultValueColumnSize = 1418;
    internal readonly int IdentifierColumnSize = 1418;
}
