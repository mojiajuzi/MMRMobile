using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.Components.Dock;
using MMRMobile.Data;
using MMRMobile.ViewModels;
using MMRMobile.Views;

namespace MMRMobile;

public partial class App : Application
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

    private void RegisterServices()
    {
        var factory = new AppDbContextFactory();
        var dbContext = factory.CreateDbContext([]);

        _services?.AddSingleton<AppDbContext>(dbContext);
        _services?.AddSingleton<MainViewModel>();
        _services?.AddSingleton<DockViewModel>();
        _services?.AddTransient<TagViewModel>();
        _services?.AddTransient<ContactViewModel>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
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
                // 使用 DI 创建 MainViewModel
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

        var dbContext = _serviceProvider?.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
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