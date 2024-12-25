using System.Linq;
using MMRMobile.Components.WorkContact;
using MMRMobile.Components.WorkPayment;
using MMRMobile.Data;
using MMRMobile.Models;

namespace MMRMobile.ViewModels;

using MMRMobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

public partial class WorkDetailViewModel : ViewModelBase, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly AppDbContext _dbContext;

    [ObservableProperty] private WorkModel _work;

    [ObservableProperty] private WorkContactViewModel _workContactViewModel;
    [ObservableProperty] private WorkPaymentViewModel _workPaymentViewModel;

    public WorkDetailViewModel(INavigationService navigationService, WorkContactViewModel workContactViewModel,
        WorkPaymentViewModel workPaymentViewModel, AppDbContext dbContext)
    {
        _navigationService = navigationService;
        _workContactViewModel = workContactViewModel;
        _workPaymentViewModel = workPaymentViewModel;
        _dbContext = dbContext;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is WorkModel work)
        {
            Work = _dbContext.Works
                .Include(w => w.WorkTags)
                .ThenInclude(wt => wt.Tag)
                .Include(w => w.WorkPayments)
                .First(w => w.Id == work.Id);

            WorkContactViewModel.Init(Work.Id);
            WorkPaymentViewModel.Initialize(Work.Id);
        }
    }

    [RelayCommand]
    private void NavigateBack()
    {
        _navigationService.NavigateBack();
    }
}