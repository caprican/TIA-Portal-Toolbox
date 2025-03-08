using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Models;

internal class DocumentStyles(Models.DocumentSettings? settings)
{
    private readonly DocumentSettings? settings = settings;

    internal Style[] SetDefault()
    {
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Document, ("Normal", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Title, ("Title", null));

        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading1, ("heading1", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading2, ("heading2", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading3, ("heading3", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading4, ("heading4", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading5, ("heading5", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Heading6, ("heading6", null));

        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.BlockTitle, ("Blocktitle", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.BlockText, ("Blocktext", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.BulletText, ("BulletText", null));
        
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.Code, ("Code", null));

        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableGrid, ("TableGrid", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableHeaderLeft, ("TableHeaderLeft", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableHeaderCenter, ("TableHeaderCenter", null));

        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableTextLeft, ("TableTextLeft", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableTextCenter, ("TableTextCenter", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.TableTextRight, ("TableTextRight", null));

        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.ListBullet, ("Listbullet", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.ListBullet2, ("Listbullet2", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.ListBullet3, ("Listbullet3", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.ListBullet4, ("Listbullet4", null));
        Helpers.DocumentHelper.Styles.Add(OpenXmlStyles.ListBullet5, ("Listbullet5", null));

        var styles = new List<Style>
        {
            // Style Normal
            new() {
                StyleId = "Standard",
                Default = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Normal" },
                UIPriority = new UIPriority { Val = 2 },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Arial", HighAnsi = "Arial", EastAsia = "Arial Unicode MS" },
                    FontSize = new FontSize { Val = "20" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "20" },
                },
            },
            // Style table
            new()
            {
                StyleId = "NormalTable",
                Default = true,
                Type = StyleValues.Table,
                StyleName = new StyleName { Val = "Normal Table" },
                UIPriority = new UIPriority { Val = 99 },
                SemiHidden = new SemiHidden(),
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleTableProperties = new StyleTableProperties
                {
                    TableIndentation = new TableIndentation { Width = 0, Type = TableWidthUnitValues.Dxa },
                    TableCellMarginDefault = new TableCellMarginDefault
                    {
                        TopMargin = new TopMargin { Width = "0", Type = TableWidthUnitValues.Dxa },
                        TableCellLeftMargin = new TableCellLeftMargin { Width = 108, Type = TableWidthValues.Dxa },
                        BottomMargin = new BottomMargin { Width = "0", Type = TableWidthUnitValues.Dxa },
                        TableCellRightMargin = new TableCellRightMargin { Width = 108, Type = TableWidthValues.Dxa }
                    }
                }
            },
            // Style Title
            new() {
                StyleId = "Title",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Title" },
                BasedOn = new BasedOn { Val = "Standard" },
                //LinkedStyle = new LinkedStyle { Val = "TitelZchn" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "240", After = "60" },
                    Justification = new Justification { Val = JustificationValues.Center },
                    OutlineLevel = new OutlineLevel { Val = 0 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { ComplexScript = "Arial" },
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript(),
                    Kern = new Kern { Val = 28u },
                    FontSize = new FontSize { Val = "32" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "32" }
                }
            },

            // Block title
            new()
            {
                StyleId = "Blocktitle",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Block title" },
                BasedOn = new BasedOn { Val = "Blocktext" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Blocktext" },
                PrimaryStyle = new PrimaryStyle(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 1134 }),
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "240", After = "100", Line = "240", LineRule = LineSpacingRuleValues.AtLeast },
                    Indentation = new Indentation { Left = "0" }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    Bold = new Bold()
                }
            },

            // Block text
            new() {
                StyleId = "Blocktext",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Block text" },
                BasedOn = new BasedOn { Val = "Standard" },
                LinkedStyle = new LinkedStyle { Val = "BlocktextChar" },
                PrimaryStyle = new PrimaryStyle(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "80", After = "80" },
                    Indentation = new Indentation { Left = "1134" },
                },
                StyleRunProperties = new StyleRunProperties
                {
                    FontSize = new FontSize { Val = "20" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "20" },
                }
            },
            new()
            {
                StyleId = "BlocktextChar",
                CustomStyle = true,
                Type = StyleValues.Character,
                StyleName = new StyleName { Val = "Blocktext Char" },
                //BasedOn = new BasedOn { Val = "Absatz-Standardschriftart" },
                LinkedStyle = new LinkedStyle { Val = "Blocktext" },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Arial", HighAnsi = "Arial", EastAsia = "Arial Unicode MS" },
                }
            },
            // Style bullet text
            new()
            {
                StyleId = "BulletText",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Bullet Text" },
                BasedOn = new BasedOn { Val = "Standard" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    //NumberingProperties = new NumberingProperties
                    //{
                    //    NumberingId = new NumberingId { Val = 0 }
                    //},
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 1491 }),
                    Indentation = new Indentation { Left = "1491" }
                }
            },


            // Style Heading
            new() {
                StyleId = "heading1",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 1" },
                BasedOn = new BasedOn { Val = "Standard" },
                NextParagraphStyle = new NextParagraphStyle { Val = "heading2" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift1Zchn" },
                PrimaryStyle = new PrimaryStyle(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                    PageBreakBefore = new PageBreakBefore(),
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingId = new NumberingId { Val = 11 }
                    },
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "80" },
                    OutlineLevel = new OutlineLevel { Val = 0 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { ComplexScript = "Arial" },
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript(),
                    Kern = new Kern { Val = 32u },
                    FontSize = new FontSize { Val = "36" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "32" }
                }
            },
            new() {
                StyleId = "heading2",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 2" },
                BasedOn = new BasedOn { Val = "Standard" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Blocktext" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift2Zchn" },
                PrimaryStyle = new PrimaryStyle(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingLevelReference = new NumberingLevelReference { Val = 1 },
                        NumberingId = new NumberingId { Val = 11 }
                    },
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "240", After = "240" },
                    OutlineLevel = new OutlineLevel { Val = 1 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { ComplexScript = "Arial" },
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript(),
                    ItalicComplexScript = new ItalicComplexScript(),
                    FontSize = new FontSize { Val = "28" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "28" }
                }
            },
            new() {
                StyleId = "heading3",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 3" },
                BasedOn = new BasedOn { Val = "Standard" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Blocktext" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift3Zchn" },
                AutoRedefine = new AutoRedefine(),
                PrimaryStyle = new PrimaryStyle(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    KeepLines = new KeepLines(),
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingLevelReference = new NumberingLevelReference { Val = 2 },
                        NumberingId = new NumberingId { Val = 11 }
                    },
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "240", After = "240" },
                    OutlineLevel = new OutlineLevel { Val = 2 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { ComplexScript = "Arial" },
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript(),
                    FontSize = new FontSize { Val = "22" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "26" }
                }
            },
            new() {
                StyleId = "heading4",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 4" },
                BasedOn = new BasedOn { Val = "berschrift3" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Blocktext" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift4Zchn" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingLevelReference = new NumberingLevelReference { Val = 3 }
                    },
                    OutlineLevel = new OutlineLevel { Val = 3 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    Bold = new Bold(),
                    BoldComplexScript = new BoldComplexScript { Val = false },
                    FontSize = new FontSize { Val = "28" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "28" }
                }
            },
            new() {
                StyleId = "heading5",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 5" },
                BasedOn = new BasedOn { Val = "heading3" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Standard" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift5Zchn" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingLevelReference = new NumberingLevelReference { Val = 4 }
                    },
                    OutlineLevel = new OutlineLevel { Val = 4 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    Bold = new Bold { Val = false },
                    BoldComplexScript = new BoldComplexScript { Val = false },
                    Italic = new Italic(),
                    ItalicComplexScript = new ItalicComplexScript()
                }
            },
            new() {
                StyleId = "heading6",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "heading 6" },
                BasedOn = new BasedOn { Val = "heading3" },
                NextParagraphStyle = new NextParagraphStyle { Val = "Standard" },
                //LinkedStyle = new LinkedStyle { Val = "berschrift6Zchn" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingLevelReference = new NumberingLevelReference { Val = 5 }
                    },
                    OutlineLevel = new OutlineLevel { Val = 5 }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    Bold = new Bold { Val = false },
                    BoldComplexScript = new BoldComplexScript { Val = false },
                    FontSize = new FontSize { Val = "22" },
                    FontSizeComplexScript = new FontSizeComplexScript { Val = "22" }
                }
            },

            // Style list bullet
            new()
            {
                StyleId = "Listbullet",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "List Bullet" },
                BasedOn = new BasedOn { Val = "Standard" },
                AutoRedefine = new AutoRedefine(),
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    NumberingProperties = new NumberingProperties
                    {
                        NumberingId = new NumberingId { Val = 1 }
                    }
                }
            },
            new()
            {
                StyleId = "Listbullet2",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "List Bullet 2" },
                BasedOn = new BasedOn { Val = "Standard" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Number, Position = 643 }),
                    Indentation = new Indentation { Left = "643", Hanging = "360" }
                }
            },
            new()
            {
                StyleId = "Listbullet3",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "List Bullet 3" },
                BasedOn = new BasedOn { Val = "Standard" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Number, Position = 926 }),
                    Indentation = new Indentation { Left = "926", Hanging = "360" }
                }
            },
            new()
            {
                StyleId = "Listbullet4",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "List Bullet 4" },
                BasedOn = new BasedOn { Val = "Standard" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Number, Position = 1209 }),
                    Indentation = new Indentation { Left = "1209", Hanging = "360" }
                }
            },
            new()
            {
                StyleId = "Listbullet5",
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "List Bullet 5" },
                BasedOn = new BasedOn { Val = "Standard" },
                UnhideWhenUsed = new UnhideWhenUsed(),
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Number, Position = 1492 }),
                    Indentation = new Indentation { Left = "1492", Hanging = "360" }
                }
            },



            // Style code
            new() {
                StyleId = "Code",
                //CustomStyle = true,
                Type = StyleValues.Character,
                StyleName = new StyleName { Val = "Code" },
                BasedOn = new BasedOn { Val = "Standard" },
                LinkedStyle = new LinkedStyle { Val = "CodeChar" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Shading = new Shading { Val = ShadingPatternValues.Clear, Fill = "D9D9D9", Color = "Auto" },
                },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Courier New", HighAnsi = "Courier New" },
                    FontSize = new FontSize { Val = "18" },
                    NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                }
            },
            new()
            {
                StyleId = "CodeChar",
                CustomStyle = true,
                Type = StyleValues.Character,
                StyleName = new StyleName { Val = "Code Char" },
                //BasedOn = new BasedOn { Val = "Absatz-Standardschriftart" },
                LinkedStyle = new LinkedStyle { Val = "Code" },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Courier New", HighAnsi = "Courier New", EastAsia = "Arial Unicode MS" },
                    FontSize = new FontSize { Val = "18" },
                    Shading = new Shading { Val = ShadingPatternValues.Clear, Fill = "D9D9D9", Color = "Auto" },
                    NoProof = new NoProof { Val = new DocumentFormat.OpenXml.OnOffValue(true) },
                }
            },

            // Table grid
            new()
            {
                StyleId = "TableGrid",
                Type = StyleValues.Table,
                StyleName = new StyleName { Val = "Table Grid" },
                BasedOn = new BasedOn { Val = "NormalTable" },
                //StyleRunProperties = new StyleRunProperties { },
                StyleTableProperties = new StyleTableProperties
                {
                    TableBorders = new TableBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" },
                        BottomBorder = new BottomBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" },
                        RightBorder = new RightBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" },
                        InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" },
                        InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Single, Space = 0u, Size = 4u, Color = "Auto" }
                    }
                }
            },

            // Style table header left
            new ()
            {
                StyleId = "TableHeaderLeft",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Table header left" },
                BasedOn = new BasedOn { Val = "Blocktext" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    KeepNext = new KeepNext(),
                    WidowControl = new WidowControl { Val = false },
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 284 },
                                    new TabStop { Val = TabStopValues.Left, Position = 567 },
                                    new TabStop { Val = TabStopValues.Left, Position = 851 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1134 }),
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "60", After = "60" },
                    Indentation = new Indentation { Left = "0" }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    Bold = new Bold(),
                    FontSize = new FontSize { Val = "18" }
                }
            },
            new()
            {
                StyleId = "TableHeaderCenter",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "Table header center" },
                BasedOn = new BasedOn { Val = "TableHeaderLeft" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center }
                }
            },

            // Table Text Left
            new()
            {
                StyleId = "TableTextLeft",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "TableText left" },
                BasedOn = new BasedOn { Val = "Blocktext" },
                LinkedStyle = new LinkedStyle { Val = "TableTextLeftChar" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 284 },
                                    new TabStop { Val = TabStopValues.Left, Position = 567 },
                                    new TabStop { Val = TabStopValues.Left, Position = 851 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1134 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1418 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1701 },
                                    new TabStop { Val = TabStopValues.Left, Position = 1985 },
                                    new TabStop { Val = TabStopValues.Left, Position = 2268 },
                                    new TabStop { Val = TabStopValues.Left, Position = 2552 },
                                    new TabStop { Val = TabStopValues.Left, Position = 2835 }),
                    SpacingBetweenLines = new SpacingBetweenLines { Before = "40", After = "40" },
                    Indentation = new Indentation { Left = "0" }
                },
                StyleRunProperties = new StyleRunProperties
                {
                    FontSize = new FontSize { Val = "18" }
                }
            },
            new()
            {
                StyleId = "TableTextLeftChar",
                CustomStyle = true,
                Type = StyleValues.Character,
                StyleName = new StyleName { Val = "TableText left Char" },
                BasedOn = new BasedOn { Val = "BlocktextChar" },
                LinkedStyle = new LinkedStyle { Val = "TableTextLeft" },
                StyleRunProperties = new StyleRunProperties
                {
                    RunFonts = new RunFonts { Ascii = "Arial", HighAnsi = "Arial", EastAsia = "Arial Unicode MS" },
                    FontSize = new FontSize { Val = "18" },
                }
            },
            // Table Text Center
            new()
            {
                StyleId = "TableTextCenter",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "TableText center" },
                BasedOn = new BasedOn { Val = "TableTextLeft" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center }
                }
            },
            // Table Text Right
            new()
            {
                StyleId = "TableTextRight",
                CustomStyle = true,
                Type = StyleValues.Paragraph,
                StyleName = new StyleName { Val = "TableText right" },
                BasedOn = new BasedOn { Val = "TableTextLeft" },
                StyleParagraphProperties = new StyleParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Right }
                }
            },
        };

        return [.. styles];
    }
}