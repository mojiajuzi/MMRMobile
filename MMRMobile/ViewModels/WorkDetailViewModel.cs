using MMRMobile.Components.WorkContact;
using MMRMobile.Components.WorkPayment;
using MMRMobile.Models;

namespace MMRMobile.ViewModels;

using MMRMobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class WorkDetailViewModel : ViewModelBase, INavigationAware
{
    private readonly INavigationService _navigationService;

    [ObservableProperty] private WorkModel _work;

    [ObservableProperty] private WorkContactViewModel _workContactViewModel;
    [ObservableProperty] private WorkPaymentViewModel _workPaymentViewModel;

    public WorkDetailViewModel(INavigationService navigationService, WorkContactViewModel workContactViewModel,
        WorkPaymentViewModel workPaymentViewModel)
    {
        _navigationService = navigationService;
        _workContactViewModel = workContactViewModel;
        _workPaymentViewModel = workPaymentViewModel;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is WorkModel work)
        {
            Work = work;
            WorkContactViewModel.Init(work.Id);
            WorkPaymentViewModel.Initialize(work.Id);
        }
    }

    [RelayCommand]
    private void NavigateBack()
    {
        _navigationService.NavigateBack();
    }
}