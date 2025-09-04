using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace TiaPortalOpenness.Models.ProjectTree;

[DebuggerDisplay("{DisplayName}")]
public abstract class Object(string Name, string? Path) : ObservableObject
{
    public string Name { get; } = Name;
    internal string? Path { get; } = Path;
    internal string? FullPath => $@"{Path}\{Name}";

    public abstract string? DisplayName { get; }

    public ObservableCollection<Object>? Items { get; set; }

    public List<CultureInfo>? Languages { get; set; }
}
