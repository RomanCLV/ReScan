using ReScanVisualizer.ViewModels;

#nullable enable

namespace ReScanVisualizer.Models
{
    public interface IScatterGraphElement
    {
        public ScatterGraphViewModel? ScatterGraph { get; set; }
        public bool BelongsToAGraph { get; }

        public void Select(bool progateToOwner);
    }
}
