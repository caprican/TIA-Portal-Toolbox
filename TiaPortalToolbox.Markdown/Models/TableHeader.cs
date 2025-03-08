namespace TiaPortalToolbox.Doc.Models;

internal struct TableHeader(string title, int width, int gridSpan = 0)
{
    public readonly string Title => title;
    public readonly int Width => width;
    public readonly int GridSpan => gridSpan;
}
