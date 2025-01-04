using System.Globalization;

namespace TiaPortalToolbox.Contracts.Services;

interface ICultureSelectorService
{
    void InitializeCulture();

    void SetCulture(string culture);

    CultureInfo GetCurrentCulture();
}
