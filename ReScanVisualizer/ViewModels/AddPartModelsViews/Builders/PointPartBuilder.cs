using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.ViewModels.AddPartModelViews.Builders
{
    public class PointPartBuilder : PartBuilderBase
    {
        public PointPartBuilder() : base()
        {
        }

        public override PartViewModelBase Build()
        {
            PointPartViewModel partPointViewModel = new PointPartViewModel(OriginBase.Base3D, ScaleFactor);
            SetCommonParameters(partPointViewModel);
            return partPointViewModel;
        }
    }
}
