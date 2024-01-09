using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ReScanVisualizer.ViewModels.AddPartModelViews.Builders;
using ReScanVisualizer.ViewModels.Parts;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class PartVisualizerViewModel : ViewModelBase
    {
        //public Model3DGroup OriginModel { get; private set; }

        public Model3D? PartModel { get; private set; }

        private PartBuilderBase? _builder;
        public PartBuilderBase? Builder
        { 
            get => _builder;
            set
            {
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                    foreach (var base3D in _builder.Bases)
                    {
                        base3D.PropertyChanged -= BuilderBase_PropertyChanged;
                    }
                }
                if (SetValue(ref _builder, value) && _builder != null)
                {
                    _builder.PropertyChanged += Builder_PropertyChanged;
                    foreach (var base3D in _builder.Bases)
                    {
                        base3D.PropertyChanged += BuilderBase_PropertyChanged;
                    }
                    BuildPartModel();
                }
            }
        }

        public PartVisualizerViewModel()
        {
            _builder = null;
            PartModel = null;
            //OriginModel = Helper3D.BuildBaseModel(new Point3D(), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), 
            //    new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)),
            //    new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)),
            //    new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)));
        }

        ~PartVisualizerViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                    foreach (var base3D in _builder.Bases)
                    {
                        base3D.PropertyChanged -= BuilderBase_PropertyChanged;
                    }
                }
                base.Dispose();
            }
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PartBuilderBase.ScaleFactor))
            {
                BuildPartModel();
            }
        }

        private void BuilderBase_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Base3DViewModel.Origin) ||
                e.PropertyName == nameof(Base3DViewModel.X) ||
                e.PropertyName == nameof(Base3DViewModel.Y) ||
                e.PropertyName == nameof(Base3DViewModel.Z) ||
                e.PropertyName == nameof(Base3DViewModel.ScaleFactor))
            {
                BuildPartModel();
            }
        }

        private void BuildPartModel()
        {
            PartViewModelBase partViewModelBase = _builder!.Build();
            partViewModelBase.Barycenter.Hide();
            PartModel = partViewModelBase.Model.Clone();
            partViewModelBase.Dispose();
            PartViewModelBase.DecreaseInstanceCreated();
            OnPropertyChanged(nameof(PartModel));
        }
    }
}
