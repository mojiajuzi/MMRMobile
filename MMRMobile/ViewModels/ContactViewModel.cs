using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.ContactPop;
using MMRMobile.Components.FilterTag;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;

namespace MMRMobile.ViewModels;

using CommunityToolkit.Mvvm.Input;

public partial class ContactViewModel : ViewModelBase
{
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private ObservableCollection<ContactModel> _contacts;
    private readonly AppDbContext _dbContext;
    private readonly INavigationService _navigationService;

    public ContactViewModel(AppDbContext dbContext, INavigationService navigationService)
    {
        _dbContext = dbContext;
        GetContacts();
        _navigationService = navigationService;
    }

    private void GetContacts()
    {
        var c = _dbContext.Contacts.AsNoTracking()
            .Include(t => t.ContactTags)
            .ThenInclude(model => model.Tag)
            .ToList();
        if (c.Count != 0)
        {
            Contacts = new ObservableCollection<ContactModel>(c);
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            SearchText = value;
            var contactList = _dbContext.Contacts.AsNoTracking().Include(t => t.ContactTags)
                .ThenInclude(model => model.Tag)
                .Where(c => c.Name.Contains(value)).ToList();
            Contacts = new ObservableCollection<ContactModel>(contactList);
        }
        else
        {
            GetContacts();
        }
    }

    [RelayCommand]
    private void OpenPopup()
    {
        _navigationService.NavigateTo<ContactPopViewModel>();
    }


    [RelayCommand]
    private void ShowContactWork()
    {
    }

    [RelayCommand]
    private void ShowPopupToUpdate(ContactModel contact)
    {
        if (contact == null) return;
        _navigationService.NavigateTo<ContactPopViewModel>(contact);
    }

    [RelayCommand]
    private void RemoveContact()
    {
    }
}