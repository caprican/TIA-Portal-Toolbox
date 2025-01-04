namespace TiaPortalToolbox.Contracts.ViewModels;

public interface INavigationAware
{
    void OnNavigatedTo(object parameter);

    void LoadCompleted();

    void OnNavigatedFrom();
}
