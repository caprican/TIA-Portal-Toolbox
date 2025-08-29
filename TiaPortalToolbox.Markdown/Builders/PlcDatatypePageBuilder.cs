using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.Extensions.Options;

using TiaPortalToolbox.Doc.Contracts.Builders;

namespace TiaPortalToolbox.Doc.Builders;

public class PlcDatatypePageBuilder(IOptions<Models.DocumentSettings> settings, WordprocessingDocument document
                                   , Core.Models.ProjectTree.Plc.Type plcItem, IEnumerable<Core.Models.ProjectTree.Plc.Object> derivedItems) : IPageBuilder
{
    private readonly Models.DocumentSettings settings = settings.Value;

    public void Build()
    {
        document.BodyAppend(new Paragraph(new Run(new Text(plcItem.Name)))
        {
            ParagraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = settings.DocumentStyle.Headings[3] }
            }
        });

        new UserDatatypeChapter(settings, document).Build(plcItem);
    }
}
