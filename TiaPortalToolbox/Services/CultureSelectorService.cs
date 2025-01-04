﻿using System.Globalization;

using TiaPortalToolbox.Contracts.Services;

namespace TiaPortalToolbox.Services;

public class CultureSelectorService(ISettingsService settingsService) : ICultureSelectorService
{
    private readonly ISettingsService settingsService = settingsService;

    public void InitializeCulture()
    {
        var culture = GetCurrentCulture();
        SetCulture(culture.Name);
    }

    public void SetCulture(string culture)
    {
        var info = CultureInfo.CreateSpecificCulture(culture);
        Thread.CurrentThread.CurrentCulture = info;
        Thread.CurrentThread.CurrentUICulture = info;
    }

    public CultureInfo GetCurrentCulture()
    {
        if (App.Current.Properties.Contains(nameof(settingsService.CurrentCulture)))
        {
            var cultureName = App.Current.Properties[nameof(settingsService.CurrentCulture)]?.ToString();
            return CultureInfo.CreateSpecificCulture(cultureName);
        }

        return CultureInfo.CurrentCulture;
    }
}
