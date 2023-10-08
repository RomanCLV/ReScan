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

            Builders.CollectionChanged += Builders_CollectionChanged;
        }

        ~ScatterGraphFilesBuilder()
        {
            Dispose();
        }

        private void Builders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Count = Builders.Count;
            CanBuild = Builders.All(x =>
            {
                if (!x.CanBuild)
                {
                    Message = x.Message;
                }
                return x.CanBuild;
            });
        }

        public void SelectFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select files",
                Filter = ".csv",
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

        /// <summary>
        /// Build an array of <see cref="ScatterGraphBuildResult"/>
        /// </summary>
        /// <returns>Return an array of <see cref="ScatterGraphBuildResult"/></returns>
        public override ScatterGraphBuildResult[] Build()
        {
            ScatterGraphBuildResult[] scatterGraphViewModels = new ScatterGraphBuildResult[Builders.Count];
            for (int i = 0; i < Builders.Count; i++)
            {
                scatterGraphViewModels[i] = Builders[i].Build()[0];
            }
            return scatterGraphViewModels;
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Builders.CollectionChanged -= Builders_CollectionChanged;
                base.Dispose();
                IsDisposed = true;
            }
        }
    }
}
