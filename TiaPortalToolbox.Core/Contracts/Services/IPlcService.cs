namespace TiaPortalToolbox.Core.Contracts.Services;

public interface IPlcService
{
    public Task GetMetaDataBlockAsync(Models.ProjectTree.Plc.Object plcItem);
}
