using CommunityToolkit.Mvvm.ComponentModel;
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

    public ContactViewModel(AppDbContext dbContext, FilterTagViewModel filterTagViewModel)
    {
        _dbContext = dbContext;
        Ftvm = filterTagViewModel;
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
    }

    [RelayCommand]
    private void PopupClose()
    {
        IsPopupOpen = false;
        ContactData = null;
    }


    [RelayCommand]
    private void ContactSubmite()
    {
    }
}