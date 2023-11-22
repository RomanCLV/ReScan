using ReScanVisualizer.ViewModels;
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

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour BaseView.xaml
    /// </summary>
    public partial class BaseView : UserControl
    {
        public BaseView()
        {
            InitializeComponent();
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BaseViewModel viewModel)
            {
                viewModel.ApplyTranslation();
            }
        }

        private void MoveToButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BaseViewModel viewModel)
            {
                viewModel.ApplyMoveTo();
            }
        }
    }
}
