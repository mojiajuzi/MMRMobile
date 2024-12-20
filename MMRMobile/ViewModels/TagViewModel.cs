using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Data;
using MMRMobile.Models;

namespace MMRMobile.ViewModels;

public partial class TagViewModel : ViewModelBase
{
    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private TagModel _tagData;
    [ObservableProperty] private ObservableCollection<TagModel> _tags;

    private readonly AppDbContext _appDbContext;

    public TagViewModel(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        GetTagList();
    }

    private void GetTagList()
    {
        var tagList = _appDbContext.Tags.AsNoTracking().OrderByDescending(t => t.DateModified).ToList();
        Tags = new ObservableCollection<TagModel>(tagList);
    }

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            GetTagList();
            return;
        }

        var tagList = _appDbContext.Tags.AsNoTracking().Where(t => t.Name.Contains(_searchText)).ToList();
        Tags = new ObservableCollection<TagModel>(tagList);
    }

    [RelayCommand]
    private void OpenPopup()
    {
        HasErrors = false;
        ErrorMessage = string.Empty;
        TagData = new TagModel();
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void PopupClose()
    {
        IsPopupOpen = false;
        HasErrors = false;
        ErrorMessage = string.Empty;
        TagData = new TagModel();
    }

    [RelayCommand]
    private void TagSubmit()
    {
        if (TagData is null) return;
        if (!TagData.Validate(out var results))
        {
            HasErrors = true;
            ErrorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            return;
        }

        //验证Tag 的名称的唯一性
        var exists =
            _appDbContext.Tags.FirstOrDefault(t =>
                t.Name.ToLower() == TagData.Name.ToLower() && t.Id != TagData.Id);
        if (exists is not null)
        {
            HasErrors = true;
            ErrorMessage = "标签已经存在";
            return;
        }

        try
        {
            if (TagData.Id > 0)
            {
                var tag = _appDbContext.Tags.FirstOrDefault(t => t.Id == TagData.Id);
                tag.Name = TagData.Name;
                tag.Active = TagData.Active;
                tag.DateModified = DateTime.UtcNow;
                _appDbContext.Tags.Update(tag);
            }
            else
            {
                _appDbContext.Tags.Add(TagData);
            }

            _appDbContext.SaveChanges();
            HasErrors = false;
            ErrorMessage = string.Empty;
            TagData = new TagModel();
            GetTagList();
            IsPopupOpen = false;
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrorMessage = ex.Message;
            return;
        }
    }

    [RelayCommand]
    private void TagUpdate(TagModel tag)
    {
        TagData = tag;
        HasErrors = false;
        ErrorMessage = string.Empty;
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void TagActive(TagModel tag)
    {
        if (tag is null) return;

        var exists = _appDbContext.Tags.FirstOrDefault(t => t.Id == tag.Id);
        if (exists is not null)
        {
            try
            {
                exists.Active = tag.Active;
                _appDbContext.Tags.Update(exists);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}