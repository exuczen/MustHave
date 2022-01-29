﻿using System.Collections.Generic;
using UnityEngine;

namespace MustHave.Utils
{
    public struct ListUtils
    {
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
            List<int> list = new List<int>();
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
            return list;
        }

        public static List<int> CreateIntList(int beg, int count)
        {
            List<int> list = new List<int>();
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
