﻿using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    public static class ListExtensionMethods
    {
        public static void Print<T>(this List<T> list, string prefix, string separator = ", ")
        {
            ListUtils.PrintList(prefix, list, separator);
        }

        public static bool CompareTo<T>(this List<T> listA, List<T> listB)
        {
            return ListUtils.CompareLists(listA, listB);
        }

        public static void RemoveDuplicates<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].Equals(list[j]))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public static void RemoveDuplicates<T>(this List<List<T>> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                List<T> listI = lists[i];
                for (int j = i + 1; j < lists.Count; j++)
                {
                    List<T> listJ = lists[j];
                    if (ListUtils.CompareLists(listI, listJ))
                    {
                        lists.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public static List<T> Join<T>(this List<List<T>> lists)
        {
            List<T> destList = new List<T>();
            foreach (var list in lists)
            {
                destList.AddRange(list);
            }
            return destList;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            List<T> pool = new List<T>(list);
            List<T> shuffled = new List<T>();
            while (pool.Count > 0)
            {
                shuffled.Add(pool.PickRandomElement());
            }
            return shuffled;
        }

        public static void RemoveOverlapsOfSortedLists(this List<List<int>> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                List<int> listI = lists[i];
                int listIBeg = listI[0];
                int listIEnd = listI[listI.Count - 1];
                if (listIEnd < listIBeg)
                {
                    (listIEnd, listIBeg) = (listIBeg, listIEnd);
                }
                for (int j = i + 1; j < lists.Count; j++)
                {
                    List<int> listJ = lists[j];
                    int listJBeg = listJ[0];
                    int listJEnd = listJ[listJ.Count - 1];
                    if (listJEnd < listJBeg)
                    {
                        (listJEnd, listJBeg) = (listJBeg, listJEnd);
                    }
                    if ((listJBeg >= listIBeg && listJBeg <= listIEnd) ||
                        (listJEnd >= listIBeg && listJEnd <= listIEnd))
                    {
                        int beg = Mathf.Min(listIBeg, listJBeg);
                        int end = Mathf.Max(listIEnd, listJEnd);
                        listI.Clear();
                        for (int x = beg; x <= end; x++)
                        {
                            listI.Add(x);
                        }
                        lists.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }
        }

        public static void RemoveEmpty<T>(this List<List<T>> lists)
        {
            lists.RemoveAll(list => list.Count == 0);
        }

        public static void InvertOrder<T>(this List<T> list)
        {
            ListUtils.InvertOrder(list, list.Count);
        }

        public static T GetRandomElementInRange<T>(this List<T> list, int min, int max)
        {
            int randIndex = Random.Range(Mathf.Clamp(min, 0, list.Count), Mathf.Clamp(max, 0, list.Count));
            return list[randIndex];
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            int randIndex = Random.Range(0, list.Count);
            return list[randIndex];
        }

        public static T PickRandomElement<T>(this List<T> list)
        {
            int randIndex = Random.Range(0, list.Count);
            T element = list[randIndex];
            list.RemoveAt(randIndex);
            return element;
        }

        public static T PickFirstElement<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                T element = list[0];
                list.RemoveAt(0);
                return element;
            }
            return default;
        }

        public static T PickLastElement<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                int lastIndex = list.Count - 1;
                T element = list[lastIndex];
                list.RemoveAt(lastIndex);
                return element;
            }
            return default;
        }

        public static void RemoveNullElements<T>(this List<T> list)
        {
            list.RemoveAll(item => item == null);
        }

        public static void AddIntRange(this List<int> list, int beg, int count)
        {
            int end = beg + count;
            for (int i = beg; i < end; i++)
            {
                list.Add(i);
            }
        }

        public static void AddIntRangeBegEnd(this List<int> list, int beg, int end)
        {
            if (end >= beg)
            {
                for (int i = beg; i <= end; i++)
                {
                    list.Add(i);
                }
            }
            else
            {
                for (int i = beg; i >= end; i--)
                {
                    list.Add(i);
                }
            }
        }
    }
}
