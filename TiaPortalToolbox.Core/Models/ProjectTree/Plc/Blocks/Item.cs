using Siemens.Engineering.SW.Blocks;
using System.Globalization;
using Siemens.Engineering;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc.Blocks;

public class Item(PlcBlock Block, string? Path) : Blocks.Object(Block, Path)
{
    public override string? DisplayName => $"{Block.Name}";

    //public List<Dictionary<PlcNetworkCommentType, Dictionary<CultureInfo, string>>> Comments { get; set; }

    public Dictionary<CultureInfo, string>? Title { get; set; }
    public Dictionary<CultureInfo, string>? Author { get; set; }
    public Dictionary<CultureInfo, string>? Comment { get; set; }
    public Dictionary<CultureInfo, string>? Function { get; set; }
    public Dictionary<CultureInfo, string>? Library { get; set; }
    public Dictionary<CultureInfo, string>? Family { get; set; }

    //public List<PlcBlockLog> Logs { get; set; }
}
