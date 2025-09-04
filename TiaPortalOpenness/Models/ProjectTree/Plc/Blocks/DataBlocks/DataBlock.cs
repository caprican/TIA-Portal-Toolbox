namespace TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.DataBlocks;

public abstract class DataBlock(Siemens.Engineering.SW.Blocks.DataBlock? Block, string? Path) : Object(Block, Path)
{
    public override string? DisplayName => $"{Block?.Name ?? string.Empty} [DB{Block?.Number}]";
}
