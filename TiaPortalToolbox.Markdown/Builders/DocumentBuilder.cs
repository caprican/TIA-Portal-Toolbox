using System.Globalization;
using System.Reflection;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;
using TiaPortalToolbox.Doc.Contracts.Factories;
using TiaPortalToolbox.Doc.Helpers;
using TiaPortalToolbox.Doc.Models;

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

    public Task CreateDocument(List<Core.Models.ProjectTree.Object> projetItems, List<Core.Models.ProjectTree.Object> derivedItems, CultureInfo culture, string path)
    {
        var tcs = new TaskCompletionSource<bool>();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                document = WordprocessingDocument.Create(path, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
                MainDocumentPart mainDocumentPart = document.AddMainDocumentPart();
                mainDocumentPart.Document = new Document();

                var styles = new DocumentStyles(settings);
                StyleDefinitionsPart? styleDefinitionsPart = DocumentHelper.AddStylesPartToPackage(document, culture);
                styleDefinitionsPart?.Styles?.Append(styles.SetDefault());

                NumberingDefinitionsPart numberingDefinitionsPart = DocumentHelper.AddNumberingDefinitionsPartToPackage(document, culture);
                numberingDefinitionsPart.Numbering.SetDefault();

                var body = mainDocumentPart.Document.AppendChild(new Body());

                //if (!string.IsNullOrEmpty(plcItem.Preamble?[culture]))
                //    body.Append(new Paragraph(new Run(new Text(plcItem.Preamble![culture]!))));

                body.Append(new Paragraph(new Run(new Text("Program blocks"))) 
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading1].Name }
                    }
                });
                foreach (var plcItem in projetItems.OfType<Core.Models.ProjectTree.Plc.Blocks.Object>())
                {
                    var derivedPLcItems = projetItems.OfType<Core.Models.ProjectTree.Plc.Type>();
                    if (pageFactory.CreatePage(plcItem, derivedPLcItems)?.Build(culture) is DocumentFormat.OpenXml.OpenXmlElement[] page)
                    {
                        body.Append(page);
                    }
                }

                body.Append(new Paragraph(new Run(new Text("PLC data types")))
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading1].Name }
                    }
                });
                foreach (var plcItem in projetItems.OfType<Core.Models.ProjectTree.Plc.Type>())
                {
                    var derivedPLcItems = projetItems.OfType<Core.Models.ProjectTree.Plc.Type>();
                    if (pageFactory.CreatePage(plcItem, derivedPLcItems)?.Build(culture) is DocumentFormat.OpenXml.OpenXmlElement[] page)
                    {
                        body.Append(page);
                    }
                }


                body.Append(new Paragraph(new Run(new Text("PLC tags & constants")))
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading1].Name }
                    }
                });

                //if (!string.IsNullOrEmpty(plcItem.Appendix?[culture]))
                //    body.Append(new Paragraph(new Run(new Text(plcItem.Appendix![culture]!))));

                body.Append(new Paragraph(new Run(new Text("Change log")))
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading2].Name }
                    }
                });

                tcs.SetResult(false);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;

    }

    private Paragraph? FindParagraphContainingText(string text)
    {
        if (document?.MainDocumentPart is null || document.MainDocumentPart.Document.Body is null) return null;

        var textElement = document.MainDocumentPart.Document.Body.Descendants<Text>().FirstOrDefault(t => t.Text.Contains(text));
        if (textElement is null) return null;

        return textElement.Ancestors<Paragraph>().FirstOrDefault();
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
