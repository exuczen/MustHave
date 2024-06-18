﻿using System.Collections.Generic;
using UnityEngine;

namespace MustHave
{
    public struct ListUtils
    {
        public static void Swap<T>(List<T> list, int a, int b)
        {
            (list[b], list[a]) = (list[a], list[b]);
        }

        public static void InvertOrder<T>(List<T> list, int count)
        {
            int halfCount = count >> 1;
            for (int i = 0; i < halfCount; i++)
            {
                Swap(list, i, count - 1 - i);
            }
        }

        public static string ToString<T>(List<T> list, string separator = ", ")
        {
            string s = "";
            for (int i = 0; i < list.Count; i++)
            {
                s += list[i].ToString() + separator;
            }
            return s;
        }

        public static List<int> CreateIntRange(int beg, int end)
        {
            var list = new List<int>();
            list.AddIntRangeBegEnd(beg, end);
            return list;
        }

        public static List<int> CreateIntList(int beg, int count)
        {
            var list = new List<int>();
            list.AddIntRange(beg, count);
            return list;
        }

        public static void PrintList<T>(string prefix, List<T> list, string separator = ", ")
        {
            Debug.Log("ListUtils.PrintList:\n" + prefix + ToString(list, separator));
        }

        public static bool CompareLists<T>(List<T> listA, List<T> listB)
        {
            if (listA.Count == listB.Count)
            {
                for (int i = 0; i < listA.Count; i++)
                {
                    if (!listA[i].Equals(listB[i]))
                        return false;
                }
                return true;
            }
            return false;
        }

        public static List<T> JoinLists<T>(List<List<T>> lists)
        {
            List<T> destList = new List<T>();
            foreach (var list in lists)
            {
                destList.AddRange(list);
            }
            return destList;
        }
    }
}
