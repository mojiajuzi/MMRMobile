using MMRMobile.Models;

namespace MMRMobile.ViewModels;

using MMRMobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class WorkDetailViewModel : ViewModelBase, INavigationAware
{
    private readonly INavigationService _navigationService;

    [ObservableProperty] private WorkModel _work;

    public WorkDetailViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is WorkModel work)
        {
            Work = work;
        }
    }

    [RelayCommand]
    private void NavigateBack()
    {
        _navigationService.NavigateBack();
    }
}