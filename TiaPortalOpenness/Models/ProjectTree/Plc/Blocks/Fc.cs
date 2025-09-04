using Siemens.Engineering.SW.Blocks;

namespace TiaPortalOpenness.Models.ProjectTree.Plc.Blocks;

public class Fc(FC? Block, string? Path) : Object(Block, Path)
{
    public override string? DisplayName => $"{Block?.Name ?? string.Empty} [FC{Block?.Number}]";

}
