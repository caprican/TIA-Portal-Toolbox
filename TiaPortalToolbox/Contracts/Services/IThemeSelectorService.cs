using TiaPortalToolbox.Models;

namespace TiaPortalToolbox.Contracts.Services;

public interface IThemeSelectorService
{
    void InitializeTheme();

    void SetTheme(AppTheme theme);

    AppTheme GetCurrentTheme();
}