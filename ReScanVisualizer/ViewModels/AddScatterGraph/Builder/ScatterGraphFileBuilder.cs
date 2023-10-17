using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphFileBuilder : ScatterGraphBuilderBase, ISelectFilesService
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (SetValue(ref _path, value))
                {
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        public string FileName => System.IO.Path.GetFileName(_path);

        private bool _containsHeader;
        public bool ContainsHeader
        {
            get => _containsHeader;
            set
            {
                if (SetValue(ref _containsHeader, value))
                {
                    OnPropertyChanged(nameof(Details));
                    State = ScatterGraphBuilderState.Ready;
                }
            }
        }

        public override string Name => $"File builder";
        public override string FullName => $"{Name} ({Path})";
        public override string Details =>
            $"{FileName}\n" +
            $"Contains header: {(ContainsHeader ? "yes" : "no")}";

        public ICommand SelectFileCommand { get; private set; }


        public ScatterGraphFileBuilder(string path, Color color, bool containsHeader) : base(color)
        {
            _path = path;
            _containsHeader = containsHeader;

            SelectFileCommand = new SelectFilesCommand(this);
        }

        /// <returns>Return a <see cref="ScatterGraphBuildResult"/> using the <see cref="ScatterGraph.ReadCSV(string, bool)"/> method.</returns>
        public override ScatterGraphBuildResult Build()
        {
            Application.Current.Dispatcher.Invoke(() => State = ScatterGraphBuilderState.Working);
            ScatterGraphBuildResult scatterGraphBuildResult;
            try
            {
                ScatterGraph graph = ScatterGraph.ReadCSV(_path, _containsHeader);
                scatterGraphBuildResult = new ScatterGraphBuildResult(graph);
                State = ScatterGraphBuilderState.Success;
            }
            catch (Exception e)
            {
                scatterGraphBuildResult = new ScatterGraphBuildResult(e);
                State = ScatterGraphBuilderState.Error;
            }
            return scatterGraphBuildResult;
        }

        public void SelectFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select file",
                Filter = "Fichiers csv (*.csv)|*.csv;|Tous les fichiers (*.*)|*.*",
                DefaultExt = ".csv",
                Multiselect = false
            };
            if (ofd.ShowDialog() != null)
            {
                Path = ofd.FileName;
            }
        }
    }
}
