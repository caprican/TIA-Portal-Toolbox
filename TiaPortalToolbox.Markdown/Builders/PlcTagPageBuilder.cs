using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;
using DocumentFormat.OpenXml.Packaging;

namespace TiaPortalToolbox.Doc.Builders;

internal class PlcTagPageBuilder(IOptions<Models.DocumentSettings> settings, WordprocessingDocument document
                                , IEnumerable<Core.Models.ProjectTree.Plc.Tag> plcItem) : IPageBuilder
{
    private readonly Models.DocumentSettings settings = settings.Value;

    public void Build()
    {
        var style = settings.DocumentStyle;

        foreach (var item in plcItem)
        {
            document.BodyAppend(new Paragraph(new Run(new Text(item.Name)))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = settings.DocumentStyle.Headings[3] }
                }
            });

            if (!string.IsNullOrEmpty(item.Descriptions?[settings.Culture]))
            {
                document.MarkdownConvert(settings, item.Descriptions![settings.Culture]);
            }

            document.BodyAppend(new Paragraph(new Run(new Text("Parameter description")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = style.MarkdownStyles["BlockTitle"] }
                }
            });
            switch (item.Name)
            {
                case string _ when item.Name.EndsWith("_Constants"):
                    document.BodyAppend(new Paragraph(new Run(new Text("Constant identifier, values and description")))
                    {
                        ParagraphProperties = new ParagraphProperties
                        {
                            ParagraphStyleId = new ParagraphStyleId { Val = style.MarkdownStyles["BlockText"] }
                        }
                    });

                    new PlcConstantChapterBuilder(settings, document).Build(item);
                    break;
                case string _ when item.Name.EndsWith("_ErrorCodes"):
                    document.BodyAppend(new Paragraph(new Run(new Text("Status & Error codes")))
                    {
                        ParagraphProperties = new ParagraphProperties
                        {
                            ParagraphStyleId = new ParagraphStyleId { Val = style.MarkdownStyles["BlockText"] }
                        }
                    });

                    new PlcErrorCodesChapterBuilder(settings, document).Build(item);
                    break;
            }

        }
    }
}
