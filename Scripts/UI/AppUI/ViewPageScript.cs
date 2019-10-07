using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MustHave.UI
{
    public class ViewPageScript : UIScript
    {
        private ViewPagerScript _viewPager = default;

        public ViewPagerScript ViewPager { get => _viewPager; set => _viewPager = value; }

        public T CreateInstance<T>(ViewPagerScript viewPager) where T : ViewPageScript
        {
            _viewPager = viewPager;
            T viewPage = Instantiate(this, _viewPager.content, false) as T;
            return viewPage;
        }
    }
}
