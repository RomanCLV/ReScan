using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReScanVisualizer.Views.AddScatterGraphViews
{
    /// <summary>
    /// Logique d'interaction pour ScatterGraphFilesBuilderView.xaml
    /// </summary>
    public partial class ScatterGraphFilesBuilderView : UserControl
    {
        public ScatterGraphFilesBuilderView()
        {
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
                        if (DataContext is  ScatterGraphFilesBuilder builder)
                        {
                            builder.Builders.Remove(selectedBuilder);
                        }
                    }
                }
            }
        }
    }
}
