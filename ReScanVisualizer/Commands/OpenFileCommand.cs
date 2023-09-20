using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels;

namespace ReScanVisualizer.Commands
{
    public class OpenFileCommand : CommandBase
    {
        private MainViewModel _mainViewModel;

        public OpenFileCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
