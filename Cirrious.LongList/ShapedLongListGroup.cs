using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cirrious.LongList
{
    public class ShapedLongListGroup<TKey, T> : ObservableCollection<T>
    {
        private readonly IComparer<T> _itemComparer;

        public TKey Key { get; private set; }

        public ShapedLongListGroup(TKey key, IComparer<T> itemComparer)
        {
            Key = key;
            _itemComparer = itemComparer;
        }

        public new void Add(T item)
        {
            // use bubblesort to insert the items
            // this could be done faster using e.g. binary insertion
            for (int i = 0; i < Count; i++)
            {
                var compareResult = _itemComparer.Compare(item, Items[i]);
                if (compareResult < 0)
                {
                    InsertItem(i, item);
                    return;
                }
            }

            // item has not been added - so use base.Add to append item to the tail of the list
            base.Add(item);
        }
    }
}