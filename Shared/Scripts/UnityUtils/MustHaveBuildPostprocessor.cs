#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.IO;
using System;

namespace MustHave
{
    public class MustHaveBuildPostprocessor
    {
#if UNITY_EDITOR
        private const string MustHaveLibName = "MustHave.dll";
        private const string MustHaveEditorLibName = "MustHaveEditor.dll";
        private const string MustHavePluginsFolderPath = @"Packages/MustHave/Shared/Plugins";
        private static readonly string MustHaveStandaloneLibPath = Path.Combine(MustHavePluginsFolderPath, "MustHaveStandalone", MustHaveLibName);
        private static readonly string MustHaveEditorLibPath = Path.Combine(MustHavePluginsFolderPath, "MustHaveEditor", MustHaveLibName);
        private static readonly string MustHaveEditorEditorLibPath = Path.Combine(MustHavePluginsFolderPath, "MustHaveEditor", MustHaveEditorLibName);
        private static string GetFullPath(string localPath) => Path.Combine(Application.dataPath, localPath);

        [MenuItem("Tools/Setup MustHave DLLs For Export")]
        public static void SetupMustHaveLibsForExport()
        {
            throw new NotImplementedException();
        }

        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var managedPluginFolderPath = Directory.GetParent(pathToBuiltProject).FullName;
            managedPluginFolderPath = Path.Combine(managedPluginFolderPath, $"{Application.productName}_Data", "Managed");
            var srcMustHaveLibPath = Path.Combine(managedPluginFolderPath, MustHaveLibName);
            var dstMustHaveLibPath = GetFullPath(MustHaveStandaloneLibPath);

            TryCopyFile(srcMustHaveLibPath, dstMustHaveLibPath);

            var projectFolderPath = Directory.GetParent(Application.dataPath).FullName;
            var assemblyFolderPath = Path.Combine(projectFolderPath, "Library", "ScriptAssemblies");
            var mustHaveAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveLibName);
            var mustHaveEditorAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveEditorLibName);

            TryCopyFile(mustHaveAssemblyLibPath, GetFullPath(MustHaveEditorLibPath));
            TryCopyFile(mustHaveEditorAssemblyLibPath, GetFullPath(MustHaveEditorEditorLibPath));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void TryCopyFile(string sourceFilePath, string destFilePath, bool deleteExisting = true)
        {
            if (File.Exists(sourceFilePath))
            {
                try
                {
                    if (deleteExisting)
                    {
                        File.Delete(destFilePath);
                    }
                    else
                    {
                        Debug.LogWarning($"TryCopyFile: File exists: {destFilePath}");
                        return;
                    }
                    File.Copy(sourceFilePath, destFilePath);
                }
                catch (System.Exception)
                {
                    throw;
                }
                Debug.Log($"Succesfully copied file: {sourceFilePath} to {destFilePath}");
            }
            else
            {
                Debug.LogWarning($"TryCopyFile: File does not exist: {sourceFilePath}");
            }
        }
#endif
    }
}
