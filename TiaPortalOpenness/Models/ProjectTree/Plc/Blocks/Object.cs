namespace TiaPortalOpenness.Models.ProjectTree.Plc.Blocks;

public abstract class Object(Siemens.Engineering.SW.Blocks.PlcBlock? PlcBlock, string? Path) : Plc.Object(PlcBlock?.Name ?? "", Path)
{
    internal Siemens.Engineering.SW.Blocks.PlcBlock? PlcBlock = PlcBlock;

    public string? Parent => PlcBlock?.Parent switch
    {
        Siemens.Engineering.SW.Blocks.PlcBlockUserGroup plcGroup => plcGroup.Name,
        _ => null
    };
}
