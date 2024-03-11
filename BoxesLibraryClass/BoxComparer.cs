namespace BoxesLibraryClass
{
    public class BoxComparer : IComparer<Box>
    {
        public int Compare(Box x, Box y) // in order to sort the boxes when trying to find a gift.
        {
            int baseComparison = x.Base.CompareTo(y.Base);
            if (baseComparison != 0) return baseComparison;
            return x.Height.CompareTo(y.Height);
        }
    }
}
