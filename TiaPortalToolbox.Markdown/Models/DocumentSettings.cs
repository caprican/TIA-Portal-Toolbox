using System.Globalization;

namespace TiaPortalToolbox.Doc.Models;

public class DocumentSettings
{
    public string ProjectPath {get;set; } = string.Empty;

    public string UserFolderPath { get; set; } = string.Empty;
    public string DocumentPath { get; set; } = string.Empty;

    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    public string TemplatePath { get; set; } = string.Empty;

    public Markdig.Renderers.Docx.Contracts.IDocumentStyle DocumentStyle { get; set; } = new Markdig.Renderers.Docx.DocumentStyles();

    public readonly int IdentColumnSize = 220;
    public readonly int DataTypeColumnSize = 1418;
    public readonly int DefaultValueColumnSize = 1418;
    public readonly int IdentifierColumnSize = 1418;
}
