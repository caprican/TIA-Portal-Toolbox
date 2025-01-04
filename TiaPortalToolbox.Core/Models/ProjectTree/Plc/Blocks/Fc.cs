using Siemens.Engineering.SW.Blocks;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc.Blocks;

public class Fc(FC Block, string? Path) : Blocks.Object(Block, Path)
{
    public override string? DisplayName => $"{Block.Name} [FC{Block.Number}]";

}
