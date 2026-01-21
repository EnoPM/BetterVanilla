using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterVanilla.Extensions;

public static class EnumerableExtensions
{
    private static readonly Random Random = new();

    extension<T>(IEnumerable<T> enumerable)
    {
        public Il2CppSystem.Collections.Generic.List<T> ToIl2CppList()
        {
            var list = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var item in enumerable)
            {
                list.Add(item);
            }
            return list;
        }
    }

    extension<T>(List<T> list)
    {
        public void Shuffle(int startAt = 0)
        {
            for (var i = startAt; i < list.Count; ++i)
            {
                var item = list[i];
                var newIndex = Random.Next(i, list.Count);
                list[i] = list[newIndex];
                list[newIndex] = item;
            }
        }

        public T PickOneRandom()
        {
            var toRemove = list[Random.Next(0, list.Count)];
            list.Remove(toRemove);
            return toRemove;
        }

        public List<T> PickRandom(int count = 1)
        {
            var picked = 0;
            var pickedItems = new List<T>();
            if (count > list.Count) return pickedItems;
            while (picked < count)
            {
                pickedItems.Add(list.PickOneRandom());
                picked++;
            }

            return pickedItems;
        }

        public T GetOneRandom()
        {
            return list[Random.Next(0, list.Count)];
        }

        public void Replace(T toRemove, T toSet)
        {
            var index = list.IndexOf(toRemove);
            if (index < 0)
            {
                list.Add(toSet);
            }
            else
            {
                list[index] = toSet;
            }
        }

        public List<T> Deduplicate(Func<T, T, bool> comparator)
        {
            var cache = new List<T>(list);
            var toRemove = new List<T>();
            foreach (var item in cache)
            {
                if (toRemove.Contains(item)) continue;
                var duplicates = cache.Where(x => comparator(x, item)).ToList();
                if (duplicates.Count > 1)
                {
                    duplicates.Remove(item);
                    toRemove.AddRange(duplicates);
                }
            }

            foreach (var item in toRemove)
            {
                cache.Remove(item);
            }

            return cache;
        }
    }

    extension<T>(Il2CppSystem.Collections.Generic.List<T> list)
    {
        public int RandomIdx()
        {
            return UnityEngine.Random.Range(0, list.Count);
        }
    }
}