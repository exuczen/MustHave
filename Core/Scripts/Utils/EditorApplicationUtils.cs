using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MustHave.Utils
{
    public struct EditorApplicationUtils
    {
#if UNITY_EDITOR
        public static bool IsCompilingOrUpdating => EditorApplication.isCompiling || EditorApplication.isUpdating;
        public static bool IsInEditMode => !EditorApplication.isPlaying;
#else
        public static bool IsCompilingOrUpdating => false;
        public static bool IsInEditMode => false;
#endif

#if UNITY_EDITOR
        public static void AddSingleActionOnEditorUpdate(Action action)
        {
            void actionOnUpdate()
            {
                EditorApplication.update -= actionOnUpdate;
                action();
            }
            EditorApplication.update += actionOnUpdate;
            EditorApplication.QueuePlayerLoopUpdate();
        }
#endif
    }
}
