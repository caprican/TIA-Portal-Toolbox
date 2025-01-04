using Siemens.Engineering.SW.Blocks;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc.Blocks;

public class Ob(OB Block, string? Path) : Blocks.Object(Block, Path)
{
    public override string? DisplayName => $"{Block.Name} [OB{Block.Number}]";
}
