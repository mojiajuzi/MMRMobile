using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.Components.Dock;
using MMRMobile.Data;
using MMRMobile.Views;

namespace MMRMobile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    [ObservableProperty] private ViewModelBase _currentView;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ChangeCurrentViewModel(string name)
    {
        CurrentView = name switch
        {
            "Tag" => _serviceProvider.GetRequiredService<TagViewModel>(),
            "Contact" => _serviceProvider.GetRequiredService<ContactViewModel>(),
            _ => CurrentView
        };
    }
}