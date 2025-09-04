using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TiaPortalOpenness.Models.ProjectTree.Plc;

[DebuggerDisplay("{DisplayName}")]
public class Item(string Name, string? Path) : Object(Name, Path)
{
    //public new ObservableCollection<Object>? Items { get; set; }

    public override string? DisplayName => Name;
}
