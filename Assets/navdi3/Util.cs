namespace navdi3
{
    using UnityEngine;
    using System.Collections.Generic;
    public class Util
    {
        public static void untiltrue(System.Func<bool> func, int maxTries = 100)
        {
            for (int i = 0; i < maxTries; i++)
            {
                if (func()) return;
            }

            throw Dj.Crashf("Util.untiltrue failed after a maximum of {0} attempts", maxTries);
        }

        // shufl: list
        public static void shufl<T>(ref List<T> a, int start = -1, int end = -1)
        {
            if (start < 0) start = 0;
            if (end < 0) end = a.Count;
            for (int i = start; i < end - 1; i++)
            {
                int j = Random.Range(i, end);
                if (i == j) continue;
                T temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }

        // shufl: array
        public static void shufl<T>(ref T[] a, int start = -1, int end = -1)
        {
            if (start < 0) start = 0;
            if (end < 0) end = a.Length;
            for (int i = start; i < end - 1; i++)
            {
                int j = Random.Range(i, end);
                if (i == j) continue;
                T temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }

        public static CollectionType findall<T, CollectionType>(ICollection<T> Items, System.Func<T, bool> FilterFN ) where CollectionType : ICollection<T>, new()
        {
            CollectionType found = new CollectionType();
            foreach (var item in Items) if (FilterFN(item)) found.Add(item);
            return found;
        }

        public static T findbest<T>(IEnumerable<T> Items, System.Func<T, float> EvalFN)
        {
            List<T> bestItems = new List<T>(); float bestValue = float.MinValue;
            foreach (var item in Items)
            {
                var value = EvalFN(item);
                if (value >= bestValue)
                {
                    if (value >= bestValue + Mathf.Epsilon)
                    {
                        bestItems.Clear();
                        bestValue = value;
                    }
                    bestItems.Add(item);
                }
            }

            if (bestItems.Count == 0) throw new System.Exception("findbest was given an empty collection. it failed");

            return bestItems[Random.Range(0, bestItems.Count)];
        }

        public static T findbest<T>(IEnumerable<T> Items, System.Func<T, int> EvalFN)
        {
            List<T> bestItems = new List<T>(); int bestValue = int.MinValue;
            foreach (var item in Items)
            {
                var value = EvalFN(item);
                if (value >= bestValue)
                {
                    if (value > bestValue)
                    {
                        bestItems.Clear();
                        bestValue = value;
                    }
                    bestItems.Add(item);
                }
            }

            if (bestItems.Count == 0) throw new System.Exception("findbest was given an empty collection. it failed");

            return bestItems[Random.Range(0, bestItems.Count)];
        }

        public static int[] shufl_order(int length)
        {
            var items = range(0, length);
            shufl(ref items);
            return items;
        }
        public static int[] range(int start_inclusive, int end_exclusive)
        {
            int[] items = new int[end_exclusive - start_inclusive];
            for (int i = 0; i < items.Length; i++) items[i] = start_inclusive + i;
            return items;
        }

        public static int tow(int a, int b, int rate)
        {
            if (a + rate < b) return a + rate; else if (a - rate > b) return a - rate; else return b;
        }
        public static float tow(float a, float b, float rate)
        {
            if (a + rate < b) return a + rate; else if (a - rate > b) return a - rate; else return b;
        }
        public static float remap(float a1, float b1, float a2, float b2, float originalValue)
        {
            // remap originalValue from "a1-b1" space to "a2-b2" space
            return (originalValue - a1) / (b1 - a1) * (b2 - a2) + a2;
        }

        public static bool boundbump(GameObject a, Vector3 move, GameObject b)
        {
            return boundbump(a.GetComponent<BoxCollider2D>().bounds, move, b.GetComponent<BoxCollider2D>().bounds);
        }
        public static bool boundbump(Bounds a, Vector3 move, Bounds b)
        {
            a.center += move;
            return a.Intersects(b);
        }
    }
}