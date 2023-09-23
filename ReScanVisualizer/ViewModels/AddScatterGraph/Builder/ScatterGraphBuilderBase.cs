using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public abstract class ScatterGraphBuilderBase : ViewModelBase
    {
        protected const uint MAX_COUNT = 10000;

        protected const double MIN_X = -10000.0;
        protected const double MAX_X =  10000.0;

        protected const double MIN_Y = -10000.0;
        protected const double MAX_Y =  10000.0;

        protected const double MIN_Z = -10000.0;
        protected const double MAX_Z =  10000.0;

        protected const double MIN_WIDTH = 1.0;
        protected const double MAX_WIDTH = 10000.0;

        protected const double MIN_HEIGTH = 1.0;
        protected const double MAX_HEIGTH = 10000.0;

        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public abstract ScatterGraphViewModel[] Build();
    }
}
