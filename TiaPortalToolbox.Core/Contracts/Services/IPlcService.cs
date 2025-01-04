namespace TiaPortalToolbox.Core.Contracts.Services;

public interface IPlcService
{
    public void GetMetaDataBlock(ref Models.ProjectTree.Object plcItem);
}
