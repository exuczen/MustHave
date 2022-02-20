using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MustHave.UI
{
    public class ViewPage : UIScript
    {
        private ViewPager _viewPager = default;

        public ViewPager ViewPager { get => _viewPager; set => _viewPager = value; }

        public T CreateInstance<T>(ViewPager viewPager) where T : ViewPage
        {
            _viewPager = viewPager;
            T viewPage = Instantiate(this, _viewPager.content, false) as T;
            return viewPage;
        }
    }
}
