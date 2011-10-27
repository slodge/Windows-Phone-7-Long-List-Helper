using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cirrious.LongList
{
    public class ShapedLongList<TKey, T> : ObservableCollection<ShapedLongListGroup<TKey, T>>
    {
        private readonly Func<T, IEnumerable<TKey>> _keyGenerators;
        private readonly IComparer<TKey> _keyComparer;
        private readonly IComparer<T> _itemComparer;
        private bool _allowEmptyGroups;

        public ShapedLongList(Func<T, IEnumerable<TKey>> keyGenerators, IComparer<TKey> keyComparer, IComparer<T> itemComparer)
        {
            _keyGenerators = keyGenerators;
            _keyComparer = keyComparer;
            _itemComparer = itemComparer;
            _allowEmptyGroups = true;
        }

        public ShapedLongList<TKey, T> ShouldAllowEmptyGroups(bool allow)
        {
            if (allow == _allowEmptyGroups)
                return this;

            _allowEmptyGroups = allow;
            if (!_allowEmptyGroups)
                RemoveEmptyGroups();

            return this;
        }

        private void RemoveEmptyGroups()
        {
            var toRemove = new List<ShapedLongListGroup<TKey, T>>();
            foreach (var group in Items)
                if (group.Count == 0)
                    toRemove.Add(group);

            foreach (var group in toRemove)
                Remove(group);
        }

        public ShapedLongList<TKey, T> AddKeys(IEnumerable<TKey> keys)
        {
            if (keys == null)
                return this;

            if (!_allowEmptyGroups)
                return this;

            foreach (var key in keys)
            {
                GetOrCreateGroup(key);
            }

            return this;
        }

        public ShapedLongList<TKey, T> Add(IEnumerable<T> newLeaves)
        {
            foreach (var newLeaf in newLeaves)
            {
                Add(newLeaf);
            }
            return this;
        }

        public ShapedLongList<TKey, T> Add(T newLeaf)
        {
            foreach (var newLeafKey in _keyGenerators(newLeaf))
            {
                Add(newLeaf, newLeafKey);
            }
            return this;
        }

        private void Add(T newLeaf, TKey newLeafKey)
        {
            var group = GetOrCreateGroup(newLeafKey);
            group.Add(newLeaf);
        }

        public ShapedLongList<TKey, T> Remove(IEnumerable<T> toRemove)
        {
            foreach (var item in toRemove)
            {
                Remove(item);
            }
            return this;
        }

        public ShapedLongList<TKey, T> Remove(T toRemove)
        {
            foreach (var key in _keyGenerators(toRemove))
            {
                Remove(toRemove, key);
            }

            return this;
        }

        public ShapedLongList<TKey, T> Remove(T toRemove, TKey key)
        {
            var group = GetGroup(key);
            if (group == null)
                return this;

            group.Remove(toRemove);

            if (!_allowEmptyGroups && group.Count == 0)
                Remove(group);

            return this;
        }

        private ShapedLongListGroup<TKey, T> GetGroup(TKey key)
        {
            return Items.FirstOrDefault(group => _keyComparer.Compare(group.Key, key) == 0);
        }

        private ShapedLongListGroup<TKey, T> GetOrCreateGroup(TKey key)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var group = Items[i];
                var keyComparison = _keyComparer.Compare(group.Key, key);
                if (keyComparison == 0)
                {
                    return group;
                }
                if (keyComparison > 0)
                {
                    return InsertNewGroup(i, key);
                }
            }

            return InsertNewGroup(Items.Count, key);
        }

        private ShapedLongListGroup<TKey, T> InsertNewGroup(int position, TKey key)
        {
            var newGroup = new ShapedLongListGroup<TKey, T>(key, _itemComparer);
            Items.Insert(position, newGroup);
            return newGroup;
        }
    }
}