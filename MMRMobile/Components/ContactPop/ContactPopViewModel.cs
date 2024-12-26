using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Components.FilterTag;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.ContactPop;

public partial class ContactPopViewModel : ViewModelBase, INavigationAware
{
    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] private ContactModel _contactData;

    [ObservableProperty] private FilterTagViewModel _ftvm;

    private readonly AppDbContext _dbContext;

    private readonly INavigationService _navigationService;

    public ContactPopViewModel(AppDbContext dbContext, FilterTagViewModel filterTagViewModel,
        INavigationService navigationService)
    {
        _dbContext = dbContext;
        _ftvm = filterTagViewModel;
        _navigationService = navigationService;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is ContactModel contactData)
        {
            ContactData = contactData;
            Ftvm.SetSelectedTag(contactData.ContactTags.Select(ct => ct.Tag).ToList());
        }
        else
        {
            ContactData = new ContactModel();
            Ftvm.SetSelectedTag([]);
        }
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

        //验证电话号码的唯一性
        var existContact =
            _dbContext.Contacts.FirstOrDefault(c => c.Phone == ContactData.Phone && c.Id != ContactData.Id);
        if (existContact != null)
        {
            HasErrors = true;
            ErrorMessage = "电话号码已存在";
            return;
        }

        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            var selectedTags = Ftvm.GetSelectedTags();
            if (ContactData.Id > 0)
            {
                var contact = _dbContext.Contacts
                    .Include(c => c.ContactTags)
                    .FirstOrDefault(c => c.Id == ContactData.Id);

                if (contact != null)
                {
                    contact.Name = ContactData.Name;
                    contact.Phone = ContactData.Phone;
                    contact.Email = ContactData.Email;
                    contact.Wechat = ContactData.Wechat;
                    contact.DateModified = DateTime.UtcNow;

                    _dbContext.ContactTags.RemoveRange(contact.ContactTags);
                    _dbContext.SaveChanges();

                    if (selectedTags.Any())
                    {
                        var contactTags = CreateContactTags(contact.Id, selectedTags);
                        _dbContext.ContactTags.AddRange(contactTags);
                        _dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                ContactData.DateCreated = DateTime.UtcNow;
                ContactData.DateModified = DateTime.UtcNow;
                _dbContext.Contacts.Add(ContactData);
                _dbContext.SaveChanges();

                if (selectedTags.Any())
                {
                    var contactTags = CreateContactTags(ContactData.Id, selectedTags);
                    _dbContext.ContactTags.AddRange(contactTags);
                    _dbContext.SaveChanges();
                }
            }

            transaction.Commit();
            ContactData = new ContactModel();
            Ftvm.SetSelectedTag([]);
            _navigationService.NavigateBack();
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }

    private List<ContactTagModel> CreateContactTags(int contactId, List<TagModel> selectedTags)
    {
        return selectedTags.Select(t => new ContactTagModel
        {
            ContactId = contactId,
            TagId = t.Id,
            CreateTime = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        }).ToList();
    }
}