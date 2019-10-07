using UnityEngine;

namespace MustHave.UI
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string title;

        public ArrayElementTitleAttribute(string title)
        {
            this.title = title;
        }
    } 
}