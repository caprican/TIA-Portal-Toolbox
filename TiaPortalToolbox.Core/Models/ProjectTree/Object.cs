using System.Collections.ObjectModel;
using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace TiaPortalToolbox.Core.Models.ProjectTree;

public abstract class Object(string Name, string? Path) : ObservableObject
{
    public string Name { get; } = Name;
    internal string? Path { get; } = Path;

    public abstract string? DisplayName { get; }

    public ObservableCollection<Object>? Items { get; set; }

    public List<CultureInfo>? Languages { get; set; }
}
