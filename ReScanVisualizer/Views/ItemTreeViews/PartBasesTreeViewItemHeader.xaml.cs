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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.Views.ItemTreeViews
{
    /// <summary>
    /// Logique d'interaction pour PartBasesTreeViewItemHeader.xaml
    /// </summary>
    public partial class PartBasesTreeViewItemHeader : UserControl
    {
        public PartBasesTreeViewItemHeader()
        {
            InitializeComponent();
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartViewModelBase viewModel)
            {
                viewModel.AreBasesHidden = !viewModel.AreBasesHidden;
            }
        }
    }
}
