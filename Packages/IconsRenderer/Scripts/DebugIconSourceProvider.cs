using System.Collections.Generic;
using UnityEngine;

namespace MustHave.IconsRenderer
{
    //[CreateAssetMenu(fileName = "DebugIconSourceProvider", menuName = "ScriptableObjects/DebugIconSourceProvider", order = 1)]
    public class DebugIconSourceProvider : ScriptableObject, IIconSourceProvider
    {
        public int IconSourceCount => iconSources.Count;

        [SerializeField]
        private List<IconSourceObject> iconSources = new List<IconSourceObject>();

        public IconSourceObject GetIconSourcePrefab(int index)
        {
            if (index >= 0 && index < iconSources.Count)
            {
                return iconSources[index];
            }
            return null;
        }
    }
}