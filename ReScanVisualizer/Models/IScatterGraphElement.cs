using ReScanVisualizer.ViewModels;

#nullable enable

namespace ReScanVisualizer.Models
{
    public interface IScatterGraphElement : ISelectable
    {
        public ScatterGraphViewModel? ScatterGraph { get; set; }
        
        public bool BelongsToAGraph { get; }

        public void Select(bool propagateToOwner);
    }
}
