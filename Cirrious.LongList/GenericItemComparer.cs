using System.Collections.Generic;

namespace Cirrious.LongList
{
    public class GenericItemComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return x.ToString().CompareTo(y.ToString());
        }
    }
}