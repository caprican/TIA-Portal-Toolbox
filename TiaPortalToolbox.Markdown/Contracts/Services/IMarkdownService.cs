namespace TiaPortalToolbox.Doc.Contracts.Services;

public interface IMarkdownService
{
    public void CreateDocX(Markdig.Syntax.MarkdownDocument md, string path);
}
