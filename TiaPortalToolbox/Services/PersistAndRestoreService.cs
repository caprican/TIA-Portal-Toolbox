using System.Collections;
using System.IO;

using Microsoft.Extensions.Options;
using TiaPortalToolbox.Contracts.Services;
using TiaPortalToolbox.Core.Contracts.Services;

using TiaPortalToolbox.Models;

namespace TiaPortalToolbox.Services;

public class PersistAndRestoreService : IPersistAndRestoreService
{
    private readonly IFileService fileService;
    private readonly AppConfig appConfig;
    private readonly string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public PersistAndRestoreService(IFileService fileService, IOptions<AppConfig> appConfig)
    {
        this.fileService = fileService;
        this.appConfig = appConfig.Value;
    }

    public void PersistData()
    {
        if (App.Current.Properties is not null)
        {
            var folderPath = Path.Combine(localAppData, appConfig.ConfigurationsFolder ?? string.Empty);
            var fileName = appConfig.AppPropertiesFileName ?? string.Empty;
            fileService.Save(folderPath, fileName, App.Current.Properties);
        }
    }

    public void RestoreData()
    {
        var folderPath = Path.Combine(localAppData, appConfig.ConfigurationsFolder ?? string.Empty);
        var fileName = appConfig.AppPropertiesFileName ?? string.Empty;
        var properties = fileService.Read<IDictionary>(folderPath, fileName);
        if (properties is not null)
        {
            foreach (DictionaryEntry property in properties)
            {
                App.Current.Properties.Add(property.Key, property.Value);
            }
        }
    }
}
