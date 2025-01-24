using System.Globalization;

namespace TiaPortalToolbox.Core.Models.ProjectTree.Plc.Blocks;

public abstract class Object(Siemens.Engineering.SW.Blocks.PlcBlock PlcBlock, string? Path) : Plc.Object(PlcBlock.Name, Path)
{
    internal Siemens.Engineering.SW.Blocks.PlcBlock PlcBlock { get; } = PlcBlock;
}
