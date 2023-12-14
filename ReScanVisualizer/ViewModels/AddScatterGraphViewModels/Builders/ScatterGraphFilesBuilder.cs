using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using ReScanVisualizer.Commands;
using ReScanVisualizer.Models;
using ReScanVisualizer.Service;
using ReScanVisualizer.ViewModels.Parts;

#nullable enable

namespace ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders
{
    public class ScatterGraphFilesBuilder : ScatterGraphBuilderBase, ISelectFilesService, IScatterGraphBuilderGroup
    {
        public IEnumerable<ScatterGraphBuilderBase> Builders { get; }

        private readonly ObservableCollection<ScatterGraphFileBuilder> _builders;

        public override IPartSource? PartsListSource
        {
            get => base.PartsListSource;
            set
            {
                if (base.PartsListSource != null && base.PartsListSource.Parts is INotifyCollectionChanged collectionChanged)
                {
                    collectionChanged.CollectionChanged -= Parts_CollectionChanged;
                }
                base.PartsListSource = value;
                if (base.PartsListSource != null && base.PartsListSource.Parts is INotifyCollectionChanged collectionChanged2)
                {
                    collectionChanged2.CollectionChanged += Parts_CollectionChanged;
                }
                UpdateAddPartCommand();
            }
        }

        public override PartViewModelBase? Part
        {
            get => base.Part;
            set
            {
                base.Part = value;
                foreach (var builder in _builders)
                {
                    builder.Part = value;
                }
            }
        }

        public CommandKey? AddPartCommand { get; private set; }

        public ICommand SelectFilesCommand { get; private set; }

        public override string Name => "Files builder";

        public override string Details => $"{_builders.Count} file{(_builders.Count == 0 ? "" : "s")}";

        public ScatterGraphFilesBuilder()
        {
            State = ScatterGraphBuilderState.Error;
            UpdateAddPartCommand();
            SelectFilesCommand = new SelectFilesCommand(this);
            _builders = new ObservableCollection<ScatterGraphFileBuilder>();
            Builders = _builders;
            _builders.CollectionChanged += Builders_CollectionChanged;
        }

        ~ScatterGraphFilesBuilder()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                AddPartCommand?.Dispose();
                _builders.CollectionChanged -= Builders_CollectionChanged;
                _builders.Clear();
                base.Dispose();
            }
        }

        private void UpdateAddPartCommand()
        {
            AddPartCommand?.Dispose();
            AddPartCommand = PartsListSource is null ?
                new CommandKey(new ActionCommand(() => { }), Key.None, ModifierKeys.None, "No parts source") :
                new CommandKey(new AddPartCommand(PartsListSource), Key.P, ModifierKeys.Control | ModifierKeys.Shift, "Add a new part");
        }

        private void Parts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Part = (PartViewModelBase)e.NewItems[e.NewItems.Count - 1];
            }
        }

        private void Builders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            State = Builders.All(x =>
            {
                if (!x.IsReady)
                {
                    Message = x.Message;
                }
                return x.IsReady;
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
                    ScatterGraphFileBuilder scatterGraphFileBuilder = new ScatterGraphFileBuilder(file, Colors.White, true)
                    {
                        Part = Part,
                        PartsListSource = PartsListSource
                    };
                    _builders.Add(scatterGraphFileBuilder);
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
    }
}
