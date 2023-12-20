using ReScanVisualizer.ViewModels.AddPartModelViews.Builders;
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

namespace ReScanVisualizer.Views.AddPartViews
{
    /// <summary>
    /// Logique d'interaction pour PartPointBuilderView.xaml
    /// </summary>
    public partial class PartPointBuilderView : UserControl
    {
        public PartPointBuilderView()
        {
            InitializeComponent();
        }

        private void NormalizeBaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartBuilderBase builder)
            {
                builder.OriginBase.Normalize();
            }
        }

        private void NormalizeXButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartBuilderBase builder)
            {
                builder.OriginBase.NormalizeX();
            }
        }

        private void NormalizeYButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartBuilderBase builder)
            {
                builder.OriginBase.NormalizeY();
            }
        }

        private void NormalizeZButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartBuilderBase builder)
            {
                builder.OriginBase.NormalizeZ();
            }
        }
    }
}
