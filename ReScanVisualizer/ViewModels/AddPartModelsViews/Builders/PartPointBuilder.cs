﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.ViewModels.Parts;

namespace ReScanVisualizer.ViewModels.AddPartModelViews.Builders
{
    public class PartPointBuilder : PartBuilderBase
    {
        public PartPointBuilder() : base()
        {
            //Name = "Point part";
        }

        public override PartViewModelBase Build()
        {
            return new PartPointViewModel(OriginBase.Base3D);
        }
    }
}
