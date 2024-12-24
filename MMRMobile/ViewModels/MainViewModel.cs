using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.Components.Dock;
using MMRMobile.Data;
using MMRMobile.Views;
using MMRMobile.Services;

namespace MMRMobile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ViewModelBase _currentView;
    [ObservableProperty] private bool _showDock = true;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.OnNavigated += NavigationService_OnNavigated;

        // 设置初始视图为TagView
        _navigationService.NavigateTo<ContactViewModel>();
    }

    private void NavigationService_OnNavigated(object sender, NavigationEventArgs e)
    {
        CurrentView = e.ViewModel;
        ShowDock = e.ShowDock;
    }

    public void ChangeCurrentViewModel(string viewName)
    {
        switch (viewName)
        {
            case "Tag":
                _navigationService.NavigateTo<TagViewModel>();
                break;
            case "Contact":
                _navigationService.NavigateTo<ContactViewModel>();
                break;
            case "Work":
                _navigationService.NavigateTo<WorkViewModel>();
                break;
        }
    }
}