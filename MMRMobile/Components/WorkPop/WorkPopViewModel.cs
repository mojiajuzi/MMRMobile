using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.FilterTag;
using MMRMobile.Components.WorkStatus;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.WorkPop;

public partial class WorkPopViewModel : ViewModelBase, INavigationAware
{
    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private WorkModel _workData = null;
    [ObservableProperty] private DateTimeOffset _selectedStartAt;
    [ObservableProperty] private DateTimeOffset _selectedEndAt;
    [ObservableProperty] private WorkStatusViewModel _workStatusView;
    [ObservableProperty] private FilterTagViewModel _filterTagView;

    private readonly AppDbContext _dbContext;

    public WorkPopViewModel(AppDbContext appDbContext, FilterTagViewModel filterTagView,
        WorkStatusViewModel workStatusView)
    {
        _dbContext = appDbContext;
        _filterTagView = filterTagView;
        _workStatusView = workStatusView;
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

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is WorkModel work)
        {
            WorkData = work;
            SelectedStartAt = work.StartAt;
            SelectedEndAt = work.EndAt;
        }
        else
        {
            WorkData = new WorkModel();
            SelectedStartAt = DateTimeOffset.UtcNow;
            SelectedEndAt = DateTimeOffset.UtcNow;
        }
    }
}