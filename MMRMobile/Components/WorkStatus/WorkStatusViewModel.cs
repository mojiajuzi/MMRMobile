using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MMRMobile.Models.Enums;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.WorkStatus;

public partial class WorkStatusViewModel : ViewModelBase
{
    [ObservableProperty] private WorkStatusEnum _selectedStatus;
    [ObservableProperty] private ObservableCollection<WorkStatusEnum> _workStatuses;

    public WorkStatusViewModel()
    {
        var list = new List<WorkStatusEnum>((WorkStatusEnum[])Enum.GetValues(typeof(WorkStatusEnum)));
        WorkStatuses = new ObservableCollection<WorkStatusEnum>(list);
    }

    public WorkStatusEnum GetSelectedStatus()
    {
        return SelectedStatus;
    }

    public void SetSelectedStatus(WorkStatusEnum status)
    {
        SelectedStatus = status;
    }
}