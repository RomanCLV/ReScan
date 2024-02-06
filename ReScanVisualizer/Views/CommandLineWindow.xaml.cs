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
using System.Windows.Shapes;

namespace ReScanVisualizer.Views
{
    /// <summary>
    /// Logique d'interaction pour CommandLineWindow.xaml
    /// </summary>
    public partial class CommandLineWindow : Window
    {
        public CommandLineWindow()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                PreviousCommandKey.Command.Execute(this);
            }
            else if (e.Key == Key.Down)
            {
                NextCommandKey.Command.Execute(this);
            }
        }
    }
}
