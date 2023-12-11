﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Service;

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public class ScatterGraphFilesBuilder : ScatterGraphBuilderBase, ISelectFilesService, IScatterGraphBuilderGroup
    {
        public IEnumerable<ScatterGraphBuilderBase> Builders { get; }

        private readonly ObservableCollection<ScatterGraphFileBuilder> _builders;

        public ICommand SelectFilesCommand { get; private set; }

        public override string Name => "Files builder";

        public override string Details => $"{_builders.Count} file{(_builders.Count == 0 ? "" : "s")}";

        public ScatterGraphFilesBuilder()
        {
            State = ScatterGraphBuilderState.Error;
            SelectFilesCommand = new SelectFilesCommand(this);
            _builders = new ObservableCollection<ScatterGraphFileBuilder>();
            Builders = _builders;

            _builders.CollectionChanged += Builders_CollectionChanged;
        }

        ~ScatterGraphFilesBuilder()
        {
            Dispose();
        }

        private void Builders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            State = Builders.All(x =>
            {
                if (!x.CanBuild)
                {
                    Message = x.Message;
                }
                return x.CanBuild;
            }) ? ScatterGraphBuilderState.Ready : ScatterGraphBuilderState.Error;
            OnPropertyChanged(nameof(Details));
        }

        public void Remove(ScatterGraphFileBuilder builder)
        {
            _builders.Remove(builder);
        }

        public void SelectFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select files",
                Filter = "Fichiers csv (*.csv)|*.csv;|Tous les fichiers (*.*)|*.*",
                DefaultExt = ".csv",
                Multiselect = true
            };
            if (ofd.ShowDialog() != null)
            {
                foreach (string file in ofd.FileNames)
                {
                    _builders.Add(new ScatterGraphFileBuilder(file, Colors.White, true));
                }
            }
        }

        /// <summary>
        /// Throw a <see cref="InvalidOperationException"/>.
        /// <br />
        /// Instead of call <see cref="Build"/>, use <see cref="Builders"/>.<see cref="ScatterGraphFileBuilder.Build"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override ScatterGraphBuildResult Build()
        {
            throw new InvalidOperationException("To build a ScatterGraphFilesBuilder, call Builder.Build()");
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _builders.CollectionChanged -= Builders_CollectionChanged;
                _builders.Clear();
                base.Dispose();
            }
        }
    }
}
