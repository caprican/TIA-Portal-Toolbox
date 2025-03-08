using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Models;

internal static class DocumentNumberings
{
    internal static void SetDefault(this DocumentFormat.OpenXml.Wordprocessing.Numbering numbering)
    {
        numbering ??= new DocumentFormat.OpenXml.Wordprocessing.Numbering();
        var abstractNumberId = numbering.Elements<AbstractNum>().Count();
        numbering.Append(
            new AbstractNum(
                new Level
                {
                    LevelIndex = 0,
                    //StartNumberingValue = new StartNumberingValue { Val = 1 },
                    NumberingFormat = new NumberingFormat { Val = NumberFormatValues.Bullet },
                    //ParagraphStyleIdInLevel = new ParagraphStyleIdInLevel { Val = Helpers.DocumentHelper.Styles[OpenXmlStyles.Document].Name },
                    LevelText = new LevelText { Val = "" },
                    //LevelJustification = new LevelJustification { Val = LevelJustificationValues.Left },
                    //PreviousParagraphProperties = new PreviousParagraphProperties
                    //{
                    //    Tabs = new Tabs(new TabStop { Val = TabStopValues.Number, Position = 360 }),
                    //    Indentation = new Indentation { Left = "360", Hanging = "360" }
                    //},
                    NumberingSymbolRunProperties = new NumberingSymbolRunProperties
                    {
                        RunFonts = new RunFonts { Ascii = "Symbol", HighAnsi = "Symbol", Hint = FontTypeHintValues.Default }
                    }
                })
            {
                AbstractNumberId = abstractNumberId,
                //MultiLevelType = new MultiLevelType { Val = MultiLevelValues.SingleLevel },
                //TemplateCode = new TemplateCode { Val = "BD8090B6" }
            }
        );
        numbering.Append(new NumberingInstance { NumberID = 1, AbstractNumId = new AbstractNumId { Val = abstractNumberId } });
        abstractNumberId++;


        numbering.Save();
    }
}
