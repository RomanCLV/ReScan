using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.Service;
using ReScanVisualizer.Views.AddScatterGraphViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphFilesBuilder : ScatterGraphBuilderBase, ISelectFilesService
    {
        public ObservableCollection<ScatterGraphFileBuilder> Builders { get; private set; }

        public ICommand SelectFilesCommand { get; private set; }

        public ScatterGraphFilesBuilder()
        {
            SelectFilesCommand = new SelectFilesCommand(this);

            Builders = new ObservableCollection<ScatterGraphFileBuilder>();
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public override ScatterGraphViewModel[] Build()
        {
            ScatterGraphViewModel[] scatterGraphViewModels = new ScatterGraphViewModel[Builders.Count];
            for (int i = 0; i < Builders.Count; i++)
            {
                scatterGraphViewModels[i] = Builders[i].Build()[0];
            }
            return scatterGraphViewModels;
        }

        public void SelectFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select files",
                DefaultExt = ".csv",
                Multiselect = true
            };
            if (ofd.ShowDialog() != null)
            {
                foreach (string file in ofd.FileNames)
                {
                    Builders.Add(new ScatterGraphFileBuilder(file, Colors.White, true));
                }
            }
        }
    }
}
