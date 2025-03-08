namespace TiaPortalToolbox.Core.Contracts.Services;

public interface IPlcService
{
    public Task GetMetaDataBlockAsync(Models.ProjectTree.Plc.Object plcItem);
    public Models.ProjectTree.Plc.Object GetMetaDataBlock(string fileName, Models.ProjectTree.Plc.Object? plcItem = null);
    public Task<Models.ProjectTree.Plc.Object?> GetItem(string name);
}
