﻿using UnityEngine;

namespace MustHave.UI
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string Title;
        public bool ShowIndex;

        public ArrayElementTitleAttribute(string title, bool showIndex = false)
        {
            Title = title;
            ShowIndex = showIndex;
        }
    }
}