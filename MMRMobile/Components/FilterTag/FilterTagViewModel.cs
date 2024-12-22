using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.FilterTag;

public partial class FilterTagViewModel : ViewModelBase
{
    [ObservableProperty] private string _searchText;

    [ObservableProperty] private ObservableCollection<TagModel> _tags;

    [ObservableProperty] private ObservableCollection<TagModel> _filteredTags;

    [ObservableProperty] private ObservableCollection<TagModel> _selectedTags;

    private readonly AppDbContext _dbContext;

    public FilterTagViewModel(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
        LoadTags();
        SetSelectedTag([]);
    }

    private void LoadTags()
    {
        var t = _dbContext.Tags.AsNoTracking().Where(t => t.Active).ToList();
        if (t.Count != 0)
        {
            Tags = new ObservableCollection<TagModel>(t);
            FilteredTags = new ObservableCollection<TagModel>(t);
        }
    }


    [RelayCommand]
    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            FilteredTags = new ObservableCollection<TagModel>(Tags);
            return;
        }

        var filteredTags = Tags.Where(t => t.Name.Contains(value)).ToList();
        FilteredTags = new ObservableCollection<TagModel>(filteredTags);
    }

    [RelayCommand]
    private void SelectTag(TagModel tag)
    {
        if (tag == null) return;
        
        var exists = SelectedTags.FirstOrDefault(t => t.Id == tag.Id);
        if (exists == null)
        {
            SelectedTags.Add(tag);
        }
        SearchText = string.Empty;  // 清空搜索文本
    }

    [RelayCommand]
    private void RemoveTag(TagModel tag)
    {
        SelectedTags.Remove(tag);
    }

    public void SetSelectedTag(List<TagModel> tags)
    {
        SelectedTags = new ObservableCollection<TagModel>(tags);
    }

    public List<TagModel> GetSelectedTags()
    {
        return SelectedTags.ToList();
    }
}