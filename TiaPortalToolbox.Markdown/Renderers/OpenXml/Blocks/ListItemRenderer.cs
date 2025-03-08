using DocumentFormat.OpenXml.Wordprocessing;

using Markdig.Syntax;

using TiaPortalToolbox.Doc.Helpers;

namespace TiaPortalToolbox.Doc.Renderers.OpenXml.Blocks;

internal class ListItemRenderer : OpenXmlObjectRenderer<ListItemBlock>
{
    protected override void Write(OpenXmlRenderer renderer, ListItemBlock obj)
    {
        //renderer.ForceCloseParagraph();

        //var listInfo = renderer.ActiveList.Peek();

        //var p = WriteAsParagraph(renderer, obj, listInfo.StyleId);
        //p.GetOrCreateProperties().NumberingProperties = new NumberingProperties
        //{
        //    NumberingId = new NumberingId() { Val = listInfo.NumberingInstance?.NumberID },
        //    NumberingLevelReference = new NumberingLevelReference { Val = listInfo.Level }
        //};

        if (renderer is null) throw new ArgumentNullException(nameof(renderer));
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var paragraph = new Paragraph();
        paragraph.SetStyle(OpenXmlStyles.Document);
        paragraph.GetOrCreateProperties().NumberingProperties = new NumberingProperties
        {
            NumberingId = new NumberingId() { Val = 1 },
            //NumberingLevelReference = new NumberingLevelReference { Val = 0 }
        };

        renderer.Push(paragraph);
        renderer.Push(new Run());
        
        renderer.WriteChildren(obj);

        renderer.Pop();
        renderer.Pop();
    }
}