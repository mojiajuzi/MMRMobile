using System;
using CommunityToolkit.Mvvm.Input;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.Dock;

public partial class DockViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public DockViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private void ChangeView(string viewName)
    {
        _mainViewModel.ChangeCurrentViewModel(viewName);
    }
}