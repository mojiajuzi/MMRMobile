using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.FilterTag;
using MMRMobile.Components.WorkStatus;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;

namespace MMRMobile.ViewModels;

public partial class WorkViewModel : ViewModelBase
{
    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private WorkModel _workData;
    [ObservableProperty] private DateTimeOffset _selectedStartAt;
    [ObservableProperty] private DateTimeOffset _selectedEndAt;
    [ObservableProperty] private WorkStatusViewModel _workStatusView;
    [ObservableProperty] private FilterTagViewModel _filterTagView;
    [ObservableProperty] private ObservableCollection<WorkModel> _works;
    private readonly AppDbContext _dbContext;
    private readonly INavigationService _navigationService;

    public WorkViewModel(AppDbContext appDbContext, WorkStatusViewModel workStatusView,
        FilterTagViewModel filterTagView, INavigationService navigationService)
    {
        _dbContext = appDbContext;
        WorkData = new WorkModel();
        WorkStatusView = workStatusView;
        FilterTagView = filterTagView;
        _navigationService = navigationService;
        GetWorks();
    }

    private void GetWorks()
    {
        var w = _dbContext.Works.AsNoTracking().Include(wt => wt.WorkTags).ThenInclude(wt => wt.Tag).ToList();
        if (w.Count != 0)
        {
            Works = new ObservableCollection<WorkModel>(w);
        }
    }

    [RelayCommand]
    private void OpenPopup()
    {
        WorkData = new WorkModel();
        SelectedStartAt = DateTimeOffset.Now;
        SelectedEndAt = DateTimeOffset.Now;
        IsPopupOpen = true;
    }


    [RelayCommand]
    private void ClosePopup()
    {
        IsPopupOpen = false;
        WorkData = new WorkModel();
    }

    [RelayCommand]
    private void WorkSubmit()
    {
        WorkData.StartAt = SelectedStartAt.LocalDateTime;
        WorkData.EndAt = SelectedEndAt.LocalDateTime;
        if (!WorkData.Validate(out var result))
        {
            HasErrors = true;
            ErrorMessage = string.Join(Environment.NewLine, result.Select(r => r.ErrorMessage));
            return;
        }

        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            WorkData.Status = WorkStatusView.GetSelectedStatus();
            var selectedTags = FilterTagView.GetSelectedTags();

            if (WorkData.Id > 0)
            {
                // 更新工作
                var work = _dbContext.Works
                    .Include(w => w.WorkTags)
                    .FirstOrDefault(w => w.Id == WorkData.Id);

                if (work != null)
                {
                    work.Name = WorkData.Name;
                    work.Description = WorkData.Description;
                    work.StartAt = WorkData.StartAt;
                    work.EndAt = WorkData.EndAt;
                    work.Status = WorkData.Status;
                    work.Funds = WorkData.Funds;
                    work.DateModified = DateTime.UtcNow;

                    // 删除现有标签关系
                    _dbContext.WorkTags.RemoveRange(work.WorkTags);
                    _dbContext.SaveChanges();

                    // 添加新的标签关系
                    if (selectedTags.Any())
                    {
                        var workTags = GetSelectTagModel(work.Id, selectedTags);
                        _dbContext.WorkTags.AddRange(workTags);
                        _dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                // 创建新工作
                WorkData.DateCreated = DateTime.UtcNow;
                WorkData.DateModified = DateTime.UtcNow;
                _dbContext.Works.Add(WorkData);
                _dbContext.SaveChanges();

                if (selectedTags.Any())
                {
                    var workTags = GetSelectTagModel(WorkData.Id, selectedTags);
                    _dbContext.WorkTags.AddRange(workTags);
                    _dbContext.SaveChanges();
                }
            }

            transaction.Commit();
            GetWorks();
            IsPopupOpen = false;
            WorkData = new WorkModel();
            FilterTagView.SetSelectedTag([]);
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }

    private IEnumerable<WorkTagModel> GetSelectTagModel(int workId, List<TagModel> tags)
    {
        return tags.Select(i => new WorkTagModel()
        {
            WorkId = WorkData.Id,
            TagId = i.Id,
            CreateTime = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        });
    }

    [RelayCommand]
    private void OpenDetails(WorkModel work)
    {
        _navigationService.NavigateTo<WorkDetailViewModel>(work);
    }

    [RelayCommand]
    private void WorkUpdate(WorkModel workModel)
    {
        WorkData = workModel;
        SelectedStartAt = workModel.StartAt;
        SelectedEndAt = workModel.EndAt;
        FilterTagView.SetSelectedTag(workModel.WorkTags.Select(wt => wt.Tag).ToList());
        WorkStatusView.SetSelectedStatus(workModel.Status);
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void WorkDelete()
    {
    }
}