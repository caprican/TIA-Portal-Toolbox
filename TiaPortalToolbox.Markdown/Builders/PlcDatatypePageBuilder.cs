using System.Globalization;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;

namespace TiaPortalToolbox.Doc.Builders;

public class PlcDatatypePageBuilder(IOptions<Models.DocumentSettings> settings, Core.Models.ProjectTree.Plc.Type plcItem, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems) : IPageBuilder
{
    private readonly Models.DocumentSettings settings = settings.Value;

    public OpenXmlElement[] Build(CultureInfo culture)
    {
        culture ??= CultureInfo.InvariantCulture;

        var body = new List<OpenXmlElement>();

        body.Add(new Paragraph(new Run(new Text(plcItem.Name)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Heading3].Name }
            }
        });

        if (!string.IsNullOrEmpty(plcItem.Descriptions?[culture]))
        {
            body.Add(new Paragraph(new Run(new Text("Description")))
            {
                ParagraphProperties = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
                }
            });
            if (Helpers.DocumentHelper.MarkdownToParagraph(plcItem.Descriptions![culture]) is IEnumerable<OpenXmlElement> xElements)
            {
                body.AddRange(xElements);
            }
        }

        body.Add(new Paragraph(new Run(new Text("Parameter description")))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.BlockTitle].Name }
            }
        });
        body.Add(new Builders.UserDatatypeChapter(settings).Build(plcItem, culture));

        return [.. body];

    }
}
