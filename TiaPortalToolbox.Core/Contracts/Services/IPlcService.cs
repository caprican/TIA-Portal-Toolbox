﻿namespace TiaPortalToolbox.Core.Contracts.Services;

public interface IPlcService
{
    public Task GetMetaDataBlockAsync(Models.ProjectTree.Plc.Object? plcItem);
    public Task GetMetaDataBlockAsync(string fileName);
    
    public Task<Models.ProjectTree.Plc.Object?> GetItem(string name);
}
