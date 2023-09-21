namespace ReScanVisualizer.Models
{
    internal class ItemIndexed<T>
    {
        public T Item { get; set; }
        public int Index { get; set; }

        public ItemIndexed()
        {
            Item = default;
            Index = 0;
        }

        public ItemIndexed(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}
