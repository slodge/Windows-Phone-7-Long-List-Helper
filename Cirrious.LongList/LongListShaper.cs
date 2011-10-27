using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirrious.LongList
{
    public static class LongListShaper
    {
        public static ShapedLongList<TKey, T> ToLongListShape<TKey, T>(
            this IEnumerable<T> items,
            Func<T, IEnumerable<TKey>> keyGenerators,
            IComparer<TKey> keyComparer = null,
            IComparer<T> itemComparer = null,
            IEnumerable<TKey> defaultKeys = null)
        {
            if (itemComparer == null)
                itemComparer = new GenericItemComparer<T>();

            if (keyComparer == null)
                keyComparer = new GenericItemComparer<TKey>();

            return new ShapedLongList<TKey, T>(keyGenerators, keyComparer, itemComparer)
                .AddKeys(defaultKeys)
                .Add(items);
        }

        public static ShapedLongList<string, T> ToFullAlphabeticalLongListShape<T>(this IEnumerable<T> items, Func<T, string> rawKeyGenerator)
        {
            var toReturn = items.ToLongListShape<string, T>((item) => ToKeyEnumerable((T itemToKey) => Alphabetical(rawKeyGenerator(itemToKey)), item));
            toReturn.AddKeys(AlphabeticalKeys());
            return toReturn;
        }

        public static ShapedLongList<TKey, T> ToLongListShape<TKey, T>(this IEnumerable<T> items,
                                                                       Func<T, TKey> keyGenerator,
                                                                       IComparer<TKey> keyComparer = null,
                                                                       IComparer<T> itemComparer = null,
                                                                       IEnumerable<TKey> defaultKeys = null)
        {
            Func<T, IEnumerable<TKey>> keyGenerators = (T item) => ToKeyEnumerable(keyGenerator, item);
            return items.ToLongListShape<TKey, T>(keyGenerators, keyComparer, itemComparer, defaultKeys);
        }

        private static IEnumerable<TKey> ToKeyEnumerable<TKey, T>(Func<T, TKey> keyGenerator, T item)
        {
            yield return keyGenerator(item);
        }

        private static IEnumerable<string> AlphabeticalKeys()
        {
            yield return "#";
            for (var ch = 'A'; ch <= 'Z'; ch++)
            {
                yield return "" + ch;
            }
        }

        private static string Alphabetical(string rawKey)
        {
            var firstChar = rawKey.ToUpper().FirstOrDefault();
            if (firstChar >= 'A' && firstChar <= 'Z')
                return "" + firstChar;

            return "#";
        }
    }
}