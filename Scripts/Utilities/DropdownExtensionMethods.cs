using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MustHave.Utilities
{
    public static class DropdownExtensionMethods
    {
        public static void SetOptions<T>(this Dropdown dropdown, string firstItemTitle, Func<T, string> itemTitle, List<T> items)
        {
            List<string> titles = new List<string>();
            if (!string.IsNullOrEmpty(firstItemTitle))
                titles.Add(firstItemTitle);
            items.ForEach(item => titles.Add(itemTitle(item)));
            dropdown.ClearOptions();
            dropdown.AddOptions(titles);
        }

        public static void SetOptions<T>(this Dropdown dropdown, string firstItemTitle, Func<T, string> itemTitle, params T[] items)
        {
            dropdown.SetOptions(firstItemTitle, itemTitle, items);
        }

        public static void SetListener(this Dropdown dropdown, UnityAction<int> onValueChanged)
        {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(onValueChanged);
        }
    } 
}