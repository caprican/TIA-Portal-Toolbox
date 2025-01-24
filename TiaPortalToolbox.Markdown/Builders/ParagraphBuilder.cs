using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Wordprocessing;

namespace TiaPortalToolbox.Doc.Builders;

internal class ParagraphBuilder(Markdig.Syntax.Block current)
{
    private readonly Markdig.Syntax.Block current = current;

    Paragraph paragraph = new();
    ParagraphProperties properties = new();
    public bool SkipNextLine = false;

    public Paragraph Build()
    {
        DoAlignment();

        DoHeaders();
        DoNumberedLists();

        paragraph.Append(properties);
        RunBuilder run = new(current, paragraph);
        return run.para;
    }

    private void DoAlignment()
    {
        //Dictionary<JustificationValues, Match> Alignment = new Dictionary<JustificationValues, Match>();
        //Alignment.Add(JustificationValues.Center, Regex.Match(current, @"^><"));
        //Alignment.Add(JustificationValues.Left, Regex.Match(current, @"^<<"));
        //Alignment.Add(JustificationValues.Right, Regex.Match(current, @"^>>"));
        //Alignment.Add(JustificationValues.Distribute, Regex.Match(current, @"^<>"));

        //foreach (KeyValuePair<JustificationValues, Match> match in Alignment)
        //{
        //    if (match.Value.Success)
        //    {
        //        properties.Append(new Justification() { Val = match.Key });
        //        current = current.Substring(2);
        //        break;
        //    }
        //}
    }

    private void DoHeaders()
    {
        //int headerLevel = current.TakeWhile((x) => x == '#').Count();

        //if (headerLevel > 0)
        //{
        //    current = current.TrimStart('#').TrimEnd('#').Trim();
        //}
        //else
        //{
        //    String sTest = Regex.Replace(nextParagraph, @"\w", "");
        //    Match isSetextHeader1 = Regex.Match(sTest, @"[=]{2,}");
        //    if (Regex.Match(sTest, @"[=]{2,}").Success)
        //    {
        //        headerLevel = 1;
        //        SkipNextLine = true;
        //    }
        //    if (Regex.Match(sTest, @"[-]{2,}").Success)
        //    {
        //        headerLevel = 2;
        //        SkipNextLine = true;
        //    }
        //}

        //if (headerLevel > 0)
        //{
        //    properties.Append(new ParagraphStyleId()
        //    {
        //        Val = "Heading" + headerLevel
        //    });
        //}
    }

    private void DoNumberedLists()
    {
        //Match numberedList = Regex.Match(current, @"^\\d\\.");

        //// Set Paragraph Styles
        //if (numberedList.Success)
        //{
        //    // Doesnt work currently, needs NumberingDefinitions adding in filecreation.cs
        //    current = current.Substring(2);
        //    NumberingProperties nPr = new NumberingProperties(
        //        new NumberingLevelReference() { Val = 0 },
        //        new NumberingId() { Val = 1 }
        //    );

        //    properties.Append(nPr);
        //}
    }
}
