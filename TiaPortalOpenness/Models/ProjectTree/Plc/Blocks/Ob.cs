using Siemens.Engineering.SW.Blocks;

namespace TiaPortalOpenness.Models.ProjectTree.Plc.Blocks;

public class Ob(OB Block, string? Path) : Object(Block, Path)
{
    public override string? DisplayName => $"{Block.Name} [OB{Block.Number}]";
}
