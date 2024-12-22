using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.FilterTag;
using MMRMobile.Data;
using MMRMobile.Models;

namespace MMRMobile.ViewModels;

using CommunityToolkit.Mvvm.Input;

public partial class ContactViewModel : ViewModelBase
{
    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private string? _searchText;


    [ObservableProperty] private ContactModel _contactData;

    [ObservableProperty] private FilterTagViewModel _ftvm;

    private readonly AppDbContext _dbContext;

    [ObservableProperty] private ObservableCollection<ContactModel> _contacts;

    public ContactViewModel(AppDbContext dbContext, FilterTagViewModel filterTagViewModel)
    {
        _dbContext = dbContext;
        Ftvm = filterTagViewModel;
        GetContacts();
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

    [RelayCommand]
    private void OpenPopup()
    {
        IsPopupOpen = true;
        ContactData = new ContactModel();
    }

    [RelayCommand]
    private void ContactSubmit()
    {
        if (!ContactData.Validate(out var result))
        {
            HasErrors = true;
            ErrorMessage = string.Join(Environment.NewLine, result.Select(r => r.ErrorMessage));
            return;
        }

        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            ContactData.DateCreated = DateTime.UtcNow;
            ContactData.DateModified = DateTime.UtcNow;
            _dbContext.Contacts.Add(ContactData);
            _dbContext.SaveChanges();

            var selectedTags = Ftvm.GetSelectedTags();
            if (selectedTags.Count != 0)
            {
                var contactTags = selectedTags.Select(t => new ContactTagModel
                {
                    ContactId = ContactData.Id,
                    TagId = t.Id,
                    CreateTime = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                });
                _dbContext.ContactTags.AddRange(contactTags);
                _dbContext.SaveChanges();
            }

            transaction.Commit();
            IsPopupOpen = false;
            ContactData = new ContactModel();
            Ftvm.SetSelectedTag([]);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void PopupClose()
    {
        IsPopupOpen = false;
        ContactData = null;
    }

    [RelayCommand]
    private void ShowContactWork()
    {
    }

    [RelayCommand]
    private void ShowPopupToUpdate()
    {
    }

    [RelayCommand]
    private void RemoveContact()
    {
    }
}