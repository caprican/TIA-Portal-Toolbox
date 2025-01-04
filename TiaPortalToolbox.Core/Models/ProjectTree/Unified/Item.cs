using System.Collections.ObjectModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Unified;

public class Item(string Name, string? Path) : Unified.Object(Name, Path)
{
    public override string? DisplayName => Name;

    public new ObservableCollection<Unified.Object>? Items { get; set; }

    public override Task<bool> ExportAsync(string path)
    {
        return Task.FromResult(false);
    }
}
