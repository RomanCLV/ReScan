using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Point3DViewModel Origin { get; private set; }

        public PartBuilderBase()
        {
            _name = "New part";
            Origin = new Point3DViewModel();
        }

        public abstract PartViewModelBase Build();
    }
}
