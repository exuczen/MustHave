using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace MustHave.Utilities
{
    public struct EditorApplicationUtils
    {
        public static bool IsCompilingOrUpdating
        {
#if UNITY_EDITOR
            get => EditorApplication.isCompiling || EditorApplication.isUpdating;
#else
            get => false;
#endif
        }

        public static bool IsInEditMode
        {
#if UNITY_EDITOR
            get => !EditorApplication.isPlaying;
#else
            get => false;
#endif
        }

        public static bool ApplicationIsPlaying
        {
            get => !IsInEditMode;
        }
    }
}