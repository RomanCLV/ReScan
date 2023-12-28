using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
            OriginBase = new Base3DViewModel(new Base3D());
            OriginBase.PropertyChanged += OriginBase_PropertyChanged;
        }

        ~PartBuilderBase()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                OriginBase.PropertyChanged -= OriginBase_PropertyChanged;
                OriginBase.Dispose();
                base.Dispose();
            }
        }

        private void OriginBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OriginBase.IsXNormalized) ||
                e.PropertyName == nameof(OriginBase.IsYNormalized) ||
                e.PropertyName == nameof(OriginBase.IsZNormalized))
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
            newBuildedPart.Name = Name;
            newBuildedPart.OriginAttachedToBarycenter = OriginAttachedToBarycenter;
        }

        public abstract PartViewModelBase Build();
    }
}
