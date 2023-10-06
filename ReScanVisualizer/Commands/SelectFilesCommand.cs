using Microsoft.Win32;
using ReScanVisualizer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace ReScanVisualizer.Commands
{
    internal class SelectFilesCommand : CommandBase
    {
        private readonly ISelectFilesService _selectFilesService;

        public SelectFilesCommand(ISelectFilesService selectFilesService)
        {
            _selectFilesService = selectFilesService;
        }

        public override void Execute(object? parameter)
        {
            _selectFilesService.SelectFiles();
        }
    }
}
