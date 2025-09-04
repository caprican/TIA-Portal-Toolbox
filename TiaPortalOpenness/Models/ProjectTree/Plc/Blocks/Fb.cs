using Siemens.Engineering.SW.Blocks;

namespace TiaPortalOpenness.Models.ProjectTree.Plc.Blocks;

public class Fb(FB? Block, string? Path) : Object(Block, Path)
{
    public override string? DisplayName => $"{Block?.Name ?? string.Empty} [FB{Block?.Number ?? 0}]";

}
