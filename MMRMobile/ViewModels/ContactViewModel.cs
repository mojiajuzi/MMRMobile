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

public partial class ContactViewModel : ViewModelBase, INavigationAware
{
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private ObservableCollection<ContactModel> _contacts;
    private readonly AppDbContext _dbContext;
    private readonly INavigationService _navigationService;

    public ContactViewModel(AppDbContext dbContext, INavigationService navigationService)
    {
        _dbContext = dbContext;
        _navigationService = navigationService;
        Contacts = new ObservableCollection<ContactModel>();
    }

    public void OnNavigatedTo(object? parameter)
    {
        // 每次导航到此页面时刷新数据
        GetContacts();
    }

    private void GetContacts()
    {
        var contacts = _dbContext.Contacts.AsNoTracking()
            .Include(t => t.ContactTags)
            .ThenInclude(model => model.Tag)
            .ToList();
        
        Contacts.Clear();
        foreach (var contact in contacts)
        {
            Contacts.Add(contact);
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var contactList = _dbContext.Contacts.AsNoTracking()
                .Include(t => t.ContactTags)
                .ThenInclude(model => model.Tag)
                .Where(c => c.Name.Contains(value))
                .ToList();

            Contacts.Clear();
            foreach (var contact in contactList)
            {
                Contacts.Add(contact);
            }
        }
        else
        {
            GetContacts();
        }
    }

    [RelayCommand]
    private void OpenPopup()
    {
        _navigationService?.NavigateTo<ContactPopViewModel>(null, false);
    }

    [RelayCommand]
    private void ShowContactWork()
    {
    }

    [RelayCommand]
    private void ShowPopupToUpdate(ContactModel contact)
    {
        if (contact == null) return;
        _navigationService.NavigateTo<ContactPopViewModel>(contact, false);
    }

    [RelayCommand]
    private void RemoveContact()
    {
    }
}