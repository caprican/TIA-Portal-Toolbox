namespace TiaPortalToolbox.Core.Contracts.Converters;

public interface IConverter
{
    public void Write(Dictionary<string, List<Dictionary<string, object>>> data, string path);
    public List<object> Read(string path);
}
