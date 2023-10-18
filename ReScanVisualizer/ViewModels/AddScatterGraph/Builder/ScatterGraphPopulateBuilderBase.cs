using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public abstract class ScatterGraphPopulateBuilderBase : ScatterGraphBuilderBase
    {
        public List<Enums> AllPlans { get; private set; }


        public ScatterGraphPopulateBuilderBase() : this(Colors.White)
        {
        }

        public ScatterGraphPopulateBuilderBase(Color color) : base(color)
        {
            AllPlans = GeneratePlan2DList();
        }

        private List<Enums> GeneratePlan2DList()
        {
            List<Enums> plans = new List<Enums>();
            foreach (Enums plan in typeof(Enums).GetEnumValues())
            {
                plans.Add(plan);
            }
            return plans;
        }
    }
}
