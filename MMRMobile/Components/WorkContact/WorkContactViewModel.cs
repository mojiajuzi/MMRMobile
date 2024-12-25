using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.Services;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.WorkContact;

public partial class WorkContactViewModel : ViewModelBase
{
    private readonly AppDbContext _dbContext;
    private int _workId;

    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private bool _isCome;
    [ObservableProperty] private ContactModel? _selectedContact;
    [ObservableProperty] private ObservableCollection<ContactModel> _contacts;
    [ObservableProperty] private ObservableCollection<WorkContactModel> _workContacts;

    public WorkContactViewModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        Contacts = new ObservableCollection<ContactModel>();
        WorkContacts = new ObservableCollection<WorkContactModel>();
    }

    public void Init(int workId)
    {
        _workId = workId;
        LoadData();
    }

    private void LoadData()
    {
        var contacts = _dbContext.Contacts.AsNoTracking().Where(c => c.Active).ToList();
        Contacts = new ObservableCollection<ContactModel>(contacts);

        var workContacts = _dbContext.WorkContacts
            .AsNoTracking()
            .Include(wc => wc.Contact)
            .Where(wc => wc.WorkId == _workId)
            .ToList();
        WorkContacts = new ObservableCollection<WorkContactModel>(workContacts);
    }

    [RelayCommand]
    private void OpenPopup()
    {
        ResetForm();
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void PopupClose()
    {
        ResetForm();
        IsPopupOpen = false;
    }

    private void ResetForm()
    {
        HasErrors = false;
        ErrorMessage = string.Empty;
        Amount = 0;
        IsCome = false;
        SelectedContact = null;
    }

    [RelayCommand]
    private void Submit()
    {
        if (SelectedContact == null)
        {
            HasErrors = true;
            ErrorMessage = "请选择联系人";
            return;
        }

        try
        {
            var exists = _dbContext.WorkContacts
                .Any(wc => wc.WorkId == _workId && wc.ContactId == SelectedContact.Id);

            if (exists)
            {
                HasErrors = true;
                ErrorMessage = "该联系人已经添加过了";
                return;
            }

            var workContact = new WorkContactModel
            {
                WorkId = _workId,
                ContactId = SelectedContact.Id,
                Amount = Amount,
                IsCome = IsCome,
                CreateTime = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            _dbContext.WorkContacts.Add(workContact);
            _dbContext.SaveChanges();

            LoadData();
            IsPopupOpen = false;
            ResetForm();
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void Update(WorkContactModel workContact)
    {
        if (workContact == null) return;

        LoadData();
        
        SelectedContact = Contacts.FirstOrDefault(c => c.Id == workContact.ContactId);
        Amount = workContact.Amount;
        IsCome = workContact.IsCome;
        HasErrors = false;
        ErrorMessage = string.Empty;
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ToggleIsCome(WorkContactModel workContact)
    {
        try
        {
            var entity = _dbContext.WorkContacts.First(wc =>
                wc.WorkId == workContact.WorkId &&
                wc.ContactId == workContact.ContactId);

            entity.IsCome = workContact.IsCome;
            entity.DateModified = DateTime.UtcNow;

            _dbContext.WorkContacts.Update(entity);
            _dbContext.SaveChanges();

            LoadData();
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void Remove(WorkContactModel workContact)
    {
        try
        {
            var entity = _dbContext.WorkContacts.First(wc =>
                wc.WorkId == workContact.WorkId &&
                wc.ContactId == workContact.ContactId);

            _dbContext.WorkContacts.Remove(entity);
            _dbContext.SaveChanges();

            LoadData();
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrorMessage = ex.Message;
        }
    }
}