using System;
using System.Collections.Generic;
using System.Linq;
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

        public Base3DViewModel OriginBase { get; private set; }

        public PartBuilderBase()
        {
            _name = "Part";
            OriginBase = new Base3DViewModel(new Base3D());
        }

        public abstract PartViewModelBase Build();
    }
}
