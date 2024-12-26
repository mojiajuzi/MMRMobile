using System;
using MMRMobile.ViewModels;

namespace MMRMobile.Services;

public interface INavigationService
{
    void NavigateTo<T>(object parameter = null, bool showDock = true) where T : ViewModelBase;
    void NavigateBack();
    event EventHandler<NavigationEventArgs> OnNavigated;
}

public class NavigationEventArgs : EventArgs
{
    public ViewModelBase ViewModel { get; }
    public bool ShowDock { get; }

    public NavigationEventArgs(ViewModelBase viewModel, bool showDock)
    {
        ViewModel = viewModel;
        ShowDock = showDock;
    }
}