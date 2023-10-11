using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable enable

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour LoadScatterGraphView.xaml
    /// </summary>
    public partial class AddScatterGraphView : Window
    {

        private ScatterGraphBuilderBase? _selectedBuilder;
        private Popup? _openedPopup;

        public AddScatterGraphView()
        {
            _selectedBuilder = null;
            _openedPopup = null;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddScatterGraphViewModel viewModel)
            {
                viewModel.AddScatterGraphBuilderCommand.Execute(null);
            }
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    if (listView.SelectedItem != null)
                    {
                        KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> selectedItem = (KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult?>)listView.SelectedItem;
                        if (DataContext is AddScatterGraphViewModel model)
                        {
                            model.Items.Remove(selectedItem);
                        }
                    }
                }
            }
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = (Popup)panel.Children[1];
                if (popup.DataContext is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                {
                    _selectedBuilder = item.Key;
                    _openedPopup = popup;
                    popup.Child.MouseLeave += Child_MouseLeave;
                    popup.IsOpen = true;
                }
                else
                {
                    _selectedBuilder = null;
                }
            }
        }

        private void Child_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_openedPopup != null)
            {
                _openedPopup.Child.MouseLeave -= Child_MouseLeave;
                _openedPopup.IsOpen = false;
            }
        }

        private void ColorPopup_Closed(object sender, EventArgs e)
        {
            _selectedBuilder = null;
        }

        private void ColorSelector_ColorChanged(object sender, Color e)
        {
            if (_selectedBuilder != null)
            {
                _selectedBuilder.Color = e;
            }
        }

        private async void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                if (b.DataContext is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                {
                    if (DataContext is AddScatterGraphViewModel model)
                    {
                        await model.BuildAsync(item);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                if (b.DataContext is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                {
                    if (DataContext is AddScatterGraphViewModel model)
                    {
                        model.Items.Remove(item);
                    }
                }
            }
        }
    }
}
