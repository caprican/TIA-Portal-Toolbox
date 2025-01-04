using System.Collections.ObjectModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Hmi;

public class Item(string Name, string? Path) : Hmi.Object(Name, Path)
{
    public new ObservableCollection<Hmi.Object>? Items { get; set; }

    public override string? DisplayName => Name;

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
