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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;

#nullable enable

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphFilesBuilderView.xaml
    /// </summary>
    public partial class ScatterGraphFilesBuilderView : UserControl
    {
        private ScatterGraphFileBuilder? _selectedBuilder;
        private Popup? _openedPopup;

        public ScatterGraphFilesBuilderView()
        {
            _selectedBuilder = null;
            _openedPopup = null;
            InitializeComponent();
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    if (listView.SelectedItem != null)
                    {
                        ScatterGraphFileBuilder selectedBuilder = (ScatterGraphFileBuilder)listView.SelectedItem;
                        if (DataContext is ScatterGraphFilesBuilder builder)
                        {
                            builder.Remove(selectedBuilder);
                        }
                    }
                }
            }
        }

        public void ColorSelector_ColorChanged(object sender, Color c)
        {
            if (_selectedBuilder != null)
            {
                _selectedBuilder.Color = c;
            }
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                StackPanel panel = (StackPanel)rect.Parent;
                Popup popup = (Popup)panel.Children[1];
                if (popup.DataContext is ScatterGraphFileBuilder builder)
                {
                    _selectedBuilder = builder;
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
    }
}
