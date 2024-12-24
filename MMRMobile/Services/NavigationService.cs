using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.ViewModels;

namespace MMRMobile.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<ViewModelBase> _navigationStack = new();

    public event EventHandler<NavigationEventArgs> OnNavigated;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<T>(object parameter = null) where T : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<T>();

        // 如果有当前视图，将其压入栈
        if (_navigationStack.Count > 0)
        {
            _navigationStack.Push(_navigationStack.Peek());
        }

        // 某些视图不显示DockView（如详情页）
        bool showDock = typeof(T) != typeof(WorkDetailViewModel);

        // 如果视图需要参数初始化
        if (viewModel is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedTo(parameter);
        }

        OnNavigated?.Invoke(this, new NavigationEventArgs(viewModel, showDock));
    }

    public void NavigateBack()
    {
        if (_navigationStack.Count > 0)
        {
            var previousViewModel = _navigationStack.Pop();
            OnNavigated?.Invoke(this, new NavigationEventArgs(previousViewModel, true));
        }
    }
}