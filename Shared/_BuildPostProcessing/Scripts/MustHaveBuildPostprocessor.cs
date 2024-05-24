#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;
#endif

namespace MustHave
{
    public static class MustHaveBuildPostprocessor
    {
#if UNITY_EDITOR
        private static readonly string ProjectFolderPath = Directory.GetParent(Application.dataPath).FullName;

        private const string ExportedPackageFolderName = "ExportedPackages";
        private const string ExportedOutlinePackageName = "MustHaveOutline.unitypackage";

        private const string MustHaveLibName = "MustHave.dll";
        private const string MustHaveEditorLibName = "MustHaveEditor.dll";
        private const string MustHavePluginsFolderPath = @"Packages/MustHave/Shared/Plugins";
        private static readonly string MustHaveStandaloneLibPath = GetPluginLibPath("MustHaveStandalone", MustHaveLibName);
        private static readonly string MustHaveEditorLibPath = GetPluginLibPath("MustHaveEditor", MustHaveLibName);
        private static readonly string MustHaveEditorEditorLibPath = GetPluginLibPath("MustHaveEditor", MustHaveEditorLibName);

        private static string GetPluginLibPath(string subfolderName, string libName) => Path.Combine(MustHavePluginsFolderPath, subfolderName, libName);
        private static string GetFullPath(string localPath) => Path.Combine(Application.dataPath, localPath);
        private static string GetAssetsPath(string localPath) => Path.Combine("Assets", localPath);

        [MenuItem("Tools/Enable MustHave DLLs platforms for export")]
        public static void EnableMustHaveLibsPlatforms()
        {
            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveStandaloneLibPath)) as PluginImporter;
            if (importer)
            {
                importer.SetCompatibleWithAnyPlatform(true);
                importer.SetExcludeEditorFromAnyPlatform(true);
                SetExcludeStandaloneFromAnyPlatform(importer, false);
                importer.SaveAndReimport();
            }
            SetMustHaveEditorLibsCompatibleWithEditor(true);
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Export MustHave Outline Package")]
        public static void ExportMustHaveOutlinePackage()
        {
            var assetPaths = new string[2] {
                "Assets/Packages/MustHave/Outline",
                "Assets/Packages/MustHave/Shared/Plugins"
            };
            var packageFolderName = ExportedPackageFolderName;
            var packageFolderPath = Path.Combine(ProjectFolderPath, packageFolderName);
            var packageFileLocalPath = Path.Combine(packageFolderName, ExportedOutlinePackageName);

            if (!Directory.Exists(packageFolderPath))
            {
                Directory.CreateDirectory(packageFolderPath);
            }
            AssetDatabase.ExportPackage(assetPaths, packageFileLocalPath, ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Disable MustHave DLLs platforms")]
        public static void DisableMustHaveLibsPlatforms()
        {
            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveStandaloneLibPath)) as PluginImporter;
            if (importer)
            {
                importer.SetCompatibleWithEditor(false);
                SetCompatibleWithStandalonePlatforms(importer, false);
                importer.SetCompatibleWithAnyPlatform(false);
                importer.SaveAndReimport();
            }
            SetMustHaveEditorLibsCompatibleWithEditor(false);
            AssetDatabase.Refresh();
        }

        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log($"OnPostprocessBuild: {target}");
            bool standaloneWindows = target.ToString().StartsWith("StandaloneWindows");
            if (standaloneWindows)
            {
                var managedPluginFolderPath = Directory.GetParent(pathToBuiltProject).FullName;
                managedPluginFolderPath = Path.Combine(managedPluginFolderPath, $"{Application.productName}_Data", "Managed");
                var srcMustHaveLibPath = Path.Combine(managedPluginFolderPath, MustHaveLibName);
                var dstMustHaveLibPath = GetFullPath(MustHaveStandaloneLibPath);

                TryCopyFile(srcMustHaveLibPath, dstMustHaveLibPath);
            }
            var assemblyFolderPath = Path.Combine(ProjectFolderPath, "Library", "ScriptAssemblies");
            var mustHaveAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveLibName);
            var mustHaveEditorAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveEditorLibName);

            TryCopyFile(mustHaveAssemblyLibPath, GetFullPath(MustHaveEditorLibPath));
            TryCopyFile(mustHaveEditorAssemblyLibPath, GetFullPath(MustHaveEditorEditorLibPath));

            //DisableMustHaveLibsPlatforms();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SetMustHaveEditorLibsCompatibleWithEditor(bool compatible)
        {
            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveEditorLibPath)) as PluginImporter;
            if (importer)
            {
                importer.SetCompatibleWithEditor(compatible);
                importer.SaveAndReimport();
            }
            importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveEditorEditorLibPath)) as PluginImporter;
            if (importer)
            {
                importer.SetCompatibleWithEditor(compatible);
                importer.SaveAndReimport();
            }
        }

        private static void SetCompatibleWithStandalonePlatforms(PluginImporter importer, bool compatible)
        {
            ForEachStandalonePlatform((importer, target, value) => importer.SetCompatibleWithPlatform(target, value), importer, compatible);
        }

        private static void SetExcludeStandaloneFromAnyPlatform(PluginImporter importer, bool exclude)
        {
            ForEachStandalonePlatform((importer, target, value) => importer.SetExcludeFromAnyPlatform(target, value), importer, exclude);
        }

        private static void ForEachStandalonePlatform(Action<PluginImporter, BuildTarget, bool> action, PluginImporter importer, bool value)
        {
            action(importer, BuildTarget.StandaloneWindows, value);
            action(importer, BuildTarget.StandaloneWindows64, value);

            //action(importer, BuildTarget.StandaloneLinux, value);
            action(importer, BuildTarget.StandaloneLinux64, value);
            //action(importer, BuildTarget.StandaloneLinuxUniversal, value);

            action(importer, BuildTarget.StandaloneOSX, value);
            //action(importer, BuildTarget.StandaloneOSXIntel, value);
            //action(importer, BuildTarget.StandaloneOSXIntel64, value);
            //action(importer, BuildTarget.StandaloneOSXUniversal, value);
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
                catch (Exception)
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
