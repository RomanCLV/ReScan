namespace ReScanVisualizer.Models
{
    public interface I3DElement : IModelisable, IHideable, ISelectable
    {
        public double ScaleFactor { get; set; }

        public bool IsMouseOver { get; set; }
    }
}
