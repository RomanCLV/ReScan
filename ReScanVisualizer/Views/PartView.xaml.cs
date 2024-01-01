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
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour PartView.xaml
    /// </summary>
    public partial class PartView : UserControl
    {
        public PartView()
        {
            InitializeComponent();
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModelBase) 
            {
                partViewModelBase.InverseIsHidden();
            }
        }

        private void BasesVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModelBase)
            {
                partViewModelBase.InverseAreBasesHidden();
            }
        }

        private void PartVisualVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModelBase)
            {
                partViewModelBase.InverseIsPartVisualHidden();
            }
        }

        private void GraphVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModelBase)
            {
                partViewModelBase.InverseAreScatterGraphesHidden();
            }
        }

        private void OriginBaseVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase partViewModelBase)
            {
                partViewModelBase.OriginBase.InverseIsHidden();
            }
        }
    }
}
