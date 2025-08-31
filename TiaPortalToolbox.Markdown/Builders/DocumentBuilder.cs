using System.Reflection;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;
using TiaPortalToolbox.Doc.Contracts.Factories;

namespace TiaPortalToolbox.Doc.Builders;

public class DocumentBuilder(IOptions<Models.DocumentSettings> settings, IPageFactory pageFactory) : IDocumentBuilder
{
    private readonly Models.DocumentSettings? settings = settings?.Value;
    private readonly IPageFactory pageFactory = pageFactory;

    private WordprocessingDocument? document;

    public void CreateDocumentWithTemplate(Core.Models.ProjectTree.Plc.Object plcItem, string templatePath, string path, bool clean = false)
    {
        File.Copy(templatePath, path);

        if (clean)
        {
            CleanContents();
        }
    }

    public void CreateDocumentFromResource(string templateResource, bool clean = false)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateResource);
        stream ??= Assembly.GetCallingAssembly().GetManifestResourceStream(templateResource);

        if (stream is null)
        {
            throw new FileNotFoundException($"Failed to load resource from {templateResource}");
        }

        var ms = new MemoryStream();
        stream.CopyTo(ms);

        document = WordprocessingDocument.Open(ms, true);

        if (clean)
        {
            CleanContents();
        }
    }

    private void CleanContents()
    {
        if (document is null) return;

        document.MainDocumentPart!.Document.Body!.RemoveAllChildren();
        if (document.MainDocumentPart?.NumberingDefinitionsPart?.Numbering is not null)
        {
            document.MainDocumentPart.NumberingDefinitionsPart.Numbering.RemoveAllChildren<NumberingInstance>();
        }
    }

    public Task CreateDocument(List<Core.Models.ProjectTree.Object> projetItems, List<Core.Models.ProjectTree.Object> derivedItems)
    {
        var tcs = new TaskCompletionSource<bool>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if(settings is null)
                {
                    return;
                }

                document = WordprocessingDocument.Create(settings.DocumentPath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                MainDocumentPart mainDocumentPart = document.AddMainDocumentPart();
                mainDocumentPart.Document = new Document();

                var style = new Models.DocumentStyles();
                settings.DocumentStyle = style;
                StyleDefinitionsPart? styleDefinitionsPart = document.AddStylesPartToPackage(settings.Culture);
                styleDefinitionsPart?.Styles?.Append(style.SetDefault());

                NumberingDefinitionsPart numberingDefinitionsPart = document.AddNumberingDefinitionsPartToPackage(settings.Culture);
                numberingDefinitionsPart.Numbering.SetDefault();

                var body = mainDocumentPart.Document.AppendChild(new Body());

                // TODO : Preamble chapter
                //if (!string.IsNullOrEmpty(plcItem.Preamble?[culture]))
                //{
                //    //body.Append(new Paragraph(new Run(new Text(plcItem.Preamble![culture]!))));
                //    document.MarkdownConvert()
                //}

                var plcBlocks = projetItems.OfType<Core.Models.ProjectTree.Plc.Blocks.Object>();
                if(plcBlocks.Any())
                {
                    mainDocumentPart.AddParagraph(Properties.Resources.ProgramBlockHeader, style.Headings[1]);
                    foreach (var plcItem in plcBlocks)
                    {
                        var derivedPLcItems = projetItems.OfType<Core.Models.ProjectTree.Plc.Type>();
                        pageFactory.CreatePage(document, plcItem, derivedPLcItems)?.Build();

                        body.Append(new Paragraph(new Run(new Break(){ Type = BreakValues.Page })));
                    }
                }

                var plcUserTypes = projetItems.OfType<Core.Models.ProjectTree.Plc.Type>();
                if (plcUserTypes.Any())
                {
                    mainDocumentPart.AddParagraph(Properties.Resources.DatatypeHeader, style.Headings[1]);
                    foreach (var plcItem in plcUserTypes)
                    {
                        var derivedPLcItems = projetItems.OfType<Core.Models.ProjectTree.Plc.Type>();
                        pageFactory.CreatePage(document, plcItem, derivedPLcItems)?.Build();
                    }
                    body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                }

                var plcTags = projetItems.OfType<Core.Models.ProjectTree.Plc.Tag>();
                if (plcTags.Any())
                {
                    mainDocumentPart.AddParagraph(Properties.Resources.TagsConstantsHeader, style.Headings[1]);
                    pageFactory.CreatePage(document, plcTags, null)?.Build();
                }

                // TODO : Appendix chapter
                //if (!string.IsNullOrEmpty(plcItem.Appendix?[culture]))
                //    body.Append(new Paragraph(new Run(new Text(plcItem.Appendix![culture]!))));

                body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                mainDocumentPart.AddParagraph(Properties.Resources.ChangeLogHeader, style.Headings[1]);

                tcs.SetResult(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;

    }

    public void Save()
    {
        if (document is not null)
        {
            document.Save();
            document?.Dispose();
        }
    }

    
}
