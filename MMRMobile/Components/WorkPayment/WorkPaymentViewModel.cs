using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MMRMobile.Data;
using MMRMobile.Models;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.WorkPayment;

public partial class WorkPaymentViewModel : ViewModelBase
{
    private readonly AppDbContext _dbContext;
    private int _workId;
    private WorkPaymentModel? _currentWorkPayment;

    [ObservableProperty] private bool _hasErrors;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isPopupOpen;
    [ObservableProperty] private string? _searchText;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private bool _isIncome;
    [ObservableProperty] private bool _hasInvoice;
    [ObservableProperty] private string _remark = string.Empty;
    [ObservableProperty] private DateTimeOffset _paymentDate = DateTimeOffset.Now;
    [ObservableProperty] private ContactModel? _selectedContact;
    [ObservableProperty] private ObservableCollection<ContactModel> _contacts;
    [ObservableProperty] private ObservableCollection<WorkPaymentModel> _workPayments;

    public WorkPaymentViewModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        Contacts = new ObservableCollection<ContactModel>();
        WorkPayments = new ObservableCollection<WorkPaymentModel>();
    }

    public void Initialize(int workId)
    {
        _workId = workId;
        LoadData();
    }

    private void LoadData()
    {
        var contacts = _dbContext.WorkContacts
            .AsNoTracking()
            .Include(wc => wc.Contact)
            .Where(wc => wc.WorkId == _workId)
            .Select(wc => wc.Contact)
            .ToList();
        Contacts = new ObservableCollection<ContactModel>(contacts);

        var workPayments = _dbContext.WorkPayments
            .AsNoTracking()
            .Include(wp => wp.Contact)
            .Where(wp => wp.WorkId == _workId)
            .OrderByDescending(wp => wp.PaymentDate)
            .ToList();
        WorkPayments = new ObservableCollection<WorkPaymentModel>(workPayments);
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
        _currentWorkPayment = null;
        HasErrors = false;
        ErrorMessage = string.Empty;
        Amount = 0;
        IsIncome = false;
        HasInvoice = false;
        Remark = string.Empty;
        PaymentDate = DateTimeOffset.Now;
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
            if (_currentWorkPayment != null)
            {
                // 更新
                var payment = _dbContext.WorkPayments.First(wp => wp.Id == _currentWorkPayment.Id);
                
                payment.ContactId = SelectedContact.Id;
                payment.Amount = Amount;
                payment.IsIncome = IsIncome;
                payment.HasInvoice = HasInvoice;
                payment.Remark = Remark;
                payment.PaymentDate = PaymentDate.DateTime;
                payment.DateModified = DateTime.UtcNow;
                
                _dbContext.WorkPayments.Update(payment);
            }
            else
            {
                // 新增
                var payment = new WorkPaymentModel
                {
                    WorkId = _workId,
                    ContactId = SelectedContact.Id,
                    Amount = Amount,
                    IsIncome = IsIncome,
                    HasInvoice = HasInvoice,
                    Remark = Remark,
                    PaymentDate = PaymentDate.DateTime,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                _dbContext.WorkPayments.Add(payment);
            }

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
    private void Update(WorkPaymentModel workPayment)
    {
        if (workPayment == null) return;

        _currentWorkPayment = workPayment;
        LoadData();
        
        SelectedContact = Contacts.FirstOrDefault(c => c.Id == workPayment.ContactId);
        Amount = workPayment.Amount;
        IsIncome = workPayment.IsIncome;
        HasInvoice = workPayment.HasInvoice;
        Remark = workPayment.Remark;
        PaymentDate = workPayment.PaymentDate;
        
        HasErrors = false;
        ErrorMessage = string.Empty;
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ToggleInvoice(WorkPaymentModel workPayment)
    {
        try
        {
            var payment = _dbContext.WorkPayments.First(wp => wp.Id == workPayment.Id);
            payment.HasInvoice = workPayment.HasInvoice;
            payment.DateModified = DateTime.UtcNow;
            
            _dbContext.WorkPayments.Update(payment);
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
    private void Remove(WorkPaymentModel workPayment)
    {
        try
        {
            var payment = _dbContext.WorkPayments.First(wp => wp.Id == workPayment.Id);
            _dbContext.WorkPayments.Remove(payment);
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