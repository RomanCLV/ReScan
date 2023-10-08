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
        public List<Plan2D> AllPlans { get; private set; }


        public ScatterGraphPopulateBuilderBase() : this(Colors.White)
        {
        }

        public ScatterGraphPopulateBuilderBase(Color color) : base(color)
        {
            AllPlans = GeneratePlan2DList();
        }

        private List<Plan2D> GeneratePlan2DList()
        {
            List<Plan2D> plans = new List<Plan2D>();
            foreach (Plan2D plan in typeof(Plan2D).GetEnumValues())
            {
                plans.Add(plan);
            }
            return plans;
        }
    }
}
