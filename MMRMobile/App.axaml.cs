using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.Components.ContactPop;
using MMRMobile.Components.Dock;
using MMRMobile.Components.FilterTag;
using MMRMobile.Components.WorkContact;
using MMRMobile.Components.WorkPayment;
using MMRMobile.Components.WorkPop;
using MMRMobile.Components.WorkStatus;
using MMRMobile.Data;
using MMRMobile.Services;
using MMRMobile.ViewModels;
using MMRMobile.Views;

namespace MMRMobile;

public class App : Application
{
    public IServiceProvider _serviceProvider { get; private set; }
    private IServiceCollection? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _services = new ServiceCollection();
        RegisterServices();
        _serviceProvider = _services.BuildServiceProvider();
    }

    private new void RegisterServices()
    {
        var factory = new AppDbContextFactory();
        var dbContext = factory.CreateDbContext([]);

        _services?.AddSingleton<INavigationService, NavigationService>();

        _services?.AddSingleton<AppDbContext>(dbContext);
        _services?.AddSingleton<MainViewModel>();
        _services?.AddSingleton<DockViewModel>();
        _services?.AddTransient<TagViewModel>();
        _services?.AddTransient<FilterTagViewModel>();
        _services?.AddTransient<WorkPopViewModel>();
        _services?.AddTransient<ContactPopViewModel>();
        _services?.AddTransient<ContactViewModel>();
        _services?.AddTransient<WorkStatusViewModel>();
        _services?.AddTransient<WorkViewModel>();
        _services?.AddTransient<WorkContactViewModel>();
        _services?.AddTransient<WorkPaymentViewModel>();
        _services?.AddTransient<WorkDetailViewModel>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbInitializer.Initialize(dbContext);
        }

        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                // 使用 DI 建 MainViewModel
                var mainViewModel = _serviceProvider!.GetRequiredService<MainViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
                break;
            }
            case ISingleViewApplicationLifetime singleViewPlatform:
            {
                var mainViewModel = _serviceProvider!.GetRequiredService<MainViewModel>();
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel
                };
                break;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}