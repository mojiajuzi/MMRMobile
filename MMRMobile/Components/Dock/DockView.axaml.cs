using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MMRMobile.ViewModels;

namespace MMRMobile.Components.Dock;

public partial class DockView : UserControl
{
    public DockView()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current)._serviceProvider.GetRequiredService<DockViewModel>();
    }
}