using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.ViewModels.AddPartModelViews.Builders
{
    public abstract class PartBuilderBase : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private bool _originAttachedToBarycenter;
        public bool OriginAttachedToBarycenter
        {
            get => _originAttachedToBarycenter;
            set => SetValue(ref _originAttachedToBarycenter, value);
        }

        private double _scaleFactor;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value <= 0.0)
                {
                    value = 1.0;
                }
                if (SetValue(ref _scaleFactor, value))
                {
                    foreach (var base3D in _bases)
                    {
                        base3D.ScaleFactor = value;
                    }
                }
            }
        }

        private readonly List<Base3DViewModel> _bases;
        public ReadOnlyCollection<Base3DViewModel> Bases { get; }

        public Base3DViewModel OriginBase { get; private set; }

        private bool _canBuild;
        public bool CanBuild
        {
            get => _canBuild;
            protected set => SetValue(ref _canBuild, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            protected set => SetValue(ref _message, value);
        }

        public PartBuilderBase()
        {
            _originAttachedToBarycenter = true;
            _name = "Part " + (PartViewModelBase.InstanceCreated + 1);
            _message = string.Empty;
            _canBuild = true;
            _scaleFactor = 1.0;
            OriginBase = new Base3DViewModel(new Base3D())
            {
                Name = "Origin base"
            };

            _bases = new List<Base3DViewModel>(1);
            AddBase(OriginBase);

            Bases = new ReadOnlyCollection<Base3DViewModel>(_bases);

            OriginBase.Base3D.OriginChanged += Base3D_OriginChanged;
        }

        ~PartBuilderBase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                OriginBase.Base3D.OriginChanged -= Base3D_OriginChanged;
                ClearBases();
                base.Dispose();
            }
        }

        protected void AddBase(Base3DViewModel base3DViewModel)
        {
            if (!_bases.Contains(base3DViewModel))
            {
                _bases.Add(base3DViewModel);
                base3DViewModel.PropertyChanged += Base_PropertyChanged;
                OnPropertyChanged(nameof(Bases));
            }
        }

        private void ClearBases()
        {
            foreach (var item in _bases)
            {
                item.PropertyChanged -= Base_PropertyChanged;
                item.Dispose();
            }
            _bases.Clear();
            OnPropertyChanged(nameof(Bases));
        }

        private void Base3D_OriginChanged(object sender, PositionEventArgs e)
        {
            Vector3D translation = e.NewPosition - e.OldPosition;
            foreach (Base3DViewModel item in _bases)
            {
                if (item.Equals(OriginBase))
                {
                    continue;
                }
                item.Translate(translation);
            }
        }

        private void Base_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Base3DViewModel.IsXNormalized) ||
                e.PropertyName == nameof(Base3DViewModel.IsYNormalized) ||
                e.PropertyName == nameof(Base3DViewModel.IsZNormalized))
            {
                UpdateCanBuildAndMessage();
            }
        }

        protected void UpdateCanBuildAndMessage()
        {
            CanBuild = ComputeCanBuild();
            UpdateMessage();
        }

        protected virtual bool ComputeCanBuild()
        {
            return
                OriginBase.IsXNormalized &&
                OriginBase.IsYNormalized &&
                OriginBase.IsZNormalized &&
                OriginBase.IsOrthogonal() &&
                OriginBase.IsDirect();
        }

        protected virtual void UpdateMessage()
        {
            if (_canBuild)
            {
                Message = string.Empty;
            }
            else
            {
                if (!OriginBase.IsXNormalized)
                {
                    Message = "X is not normalized.";
                }
                else if (!OriginBase.IsYNormalized)
                {
                    Message = "Y is not normalized.";
                }
                else if (!OriginBase.IsZNormalized)
                {
                    Message = "Z is not normalized.";
                }
                else if (!OriginBase.IsOrthogonal())
                {
                    Message = "Base is not orthogonal.";
                }
                else if (!OriginBase.IsDirect())
                {
                    Message = "Base is not direct.";
                }
            }
        }

        protected void SetCommonParameters(PartViewModelBase newBuildedPart)
        {
            newBuildedPart.Name = _name;
            newBuildedPart.OriginAttachedToBarycenter = _originAttachedToBarycenter;
            newBuildedPart.ScaleFactor = _scaleFactor;
        }

        public abstract PartViewModelBase Build();
    }
}
