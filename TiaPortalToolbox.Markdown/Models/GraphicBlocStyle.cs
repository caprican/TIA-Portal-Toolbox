namespace TiaPortalToolbox.Doc.Models;

internal record GraphicBlocStyle : Markdig.Renderers.Docx.Contracts.IDocumentStyleTable
{
    public string StyleName { get; set; } = string.Empty;
    public Markdig.Renderers.Docx.Contracts.IDocumentStyleTable? Header { get; set; }

    public string BorderColor { get; set; } = string.Empty;
    public uint BorderSpace { get; set; }
    public uint BorderSize { get; set; }

    public string ShadingColor { get; set; } = string.Empty;
    public string ShadingFill { get; set; } = string.Empty;
    public string ShadingFillLock { get; internal set; } = "EEEEEE";
    public string ShadingFillSafety { get; internal set; } = "FFFF33";
    public string ColorHidden { get; internal set; } = "777777";

    public uint TableSize { get; internal set; }

    public uint FunctionBlockNameSize { get; internal set; }
    public uint FunctionBlockConnectorSize { get; internal set; }
    public uint FunctionBlockTypeSize { get; internal set; }
}
