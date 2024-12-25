using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.FilterTag;
using MMRMobile.Components.WorkPop;
using MMRMobile.Components.WorkStatus;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;

namespace MMRMobile.ViewModels;

public partial class WorkViewModel : ViewModelBase
{
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private WorkModel _workData;
    [ObservableProperty] private ObservableCollection<WorkModel> _works;
    private readonly AppDbContext _dbContext;
    private readonly INavigationService _navigationService;

    public WorkViewModel(AppDbContext appDbContext, INavigationService navigationService)
    {
        _dbContext = appDbContext;
        WorkData = new WorkModel();
        _navigationService = navigationService;
        GetWorks();
    }

    private void GetWorks()
    {
        var w = _dbContext.Works.AsNoTracking().Include(wt => wt.WorkTags).ThenInclude(wt => wt.Tag).Include(w => w.WorkPayments).ToList();
        if (w.Count != 0)
        {
            Works = new ObservableCollection<WorkModel>(w);
        }
    }

    [RelayCommand]
    private void OpenPopup()
    {
        _navigationService.NavigateTo<WorkPopViewModel>();
    }


    [RelayCommand]
    private void OpenDetails(WorkModel work)
    {
        _navigationService.NavigateTo<WorkDetailViewModel>(work);
    }

    [RelayCommand]
    private void WorkUpdate(WorkModel workModel)
    {
        _navigationService.NavigateTo<WorkPopViewModel>(workModel);
    }

    [RelayCommand]
    private void WorkDelete()
    {
    }
}