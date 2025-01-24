using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using TiaPortalToolbox.Doc.Builders;
using TiaPortalToolbox.Doc.Contracts.Services;

namespace TiaPortalToolbox.Doc.Services;

public class MarkdownService : IMarkdownService
{
    private CultureInfo culture;


    public void CreateDocX(Markdig.Syntax.MarkdownDocument md, string path)
    {
        Body body = new();

        var SkipNextLine = false;
        foreach (var line in md)
        {
            //if (SkipNextLine)
            //{
            //    SkipNextLine = !SkipNextLine;
            //    continue;
            //}

            ParagraphBuilder paragraph = new(line);
            //SkipNextLine = paragraph.SkipNextLine;
            body.Append(paragraph.Build());
        }

        //DocumentBuilder file = new(body);
        //file.SaveTo(path);
    }
}
