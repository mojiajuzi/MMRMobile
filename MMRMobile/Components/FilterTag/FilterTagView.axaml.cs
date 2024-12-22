using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MMRMobile.Models;

namespace MMRMobile.Components.FilterTag;

public partial class FilterTagView : UserControl
{
    public FilterTagView()
    {
        InitializeComponent();
    }

    private void TagSearchBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not FilterTagViewModel viewModel || e.AddedItems.Count <= 0) return;
        if (e.AddedItems[0] is TagModel selectedTag)
        {
            viewModel.SelectTagCommand.Execute(selectedTag);
        }

        // 清除AutoCompleteBox的选择
        if (sender is AutoCompleteBox autoComplete)
        {
            autoComplete.SelectedItem = null;
        }
    }
}