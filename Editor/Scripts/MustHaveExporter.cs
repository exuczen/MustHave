using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;
using MustHave.Utils;

namespace MustHave
{
    public static class MustHaveExporter
    {
        private static readonly string ProjectFolderPath = BuildUtils.ProjectFolderPath;

        private const string ExportedPackageFolderName = "ExportedPackages";

        private const string SharedFolderPath = @"Packages/MustHave/Shared";
        private const string SharedHiddenFolderPath = @"Packages/MustHaveHidden~/Shared";
        private static readonly string SharedFolderMetaPath = $"{SharedFolderPath}.meta";
        private static readonly string SharedHiddenFolderMetaPath = $"{SharedHiddenFolderPath}.meta";
        private const string MustHaveLibName = "MustHave.dll";
        private const string MustHaveEditorLibName = "MustHaveEditor.dll";
        private const string MustHavePluginsFolderPath = @"Packages/MustHave/SharedPlugins";
        private static readonly string MustHaveStandaloneLibPath = GetPluginLibPath("MustHaveStandalone", MustHaveLibName);
        private static readonly string MustHaveEditorLibPath = GetPluginLibPath("MustHaveEditor", MustHaveLibName);
        private static readonly string MustHaveEditorEditorLibPath = GetPluginLibPath("MustHaveEditor", MustHaveEditorLibName);

        private static string GetPluginLibPath(string subfolderName, string libName) => Path.Combine(MustHavePluginsFolderPath, subfolderName, libName);
        private static string GetFullPath(string localPath) => Path.Combine(Application.dataPath, localPath);
        private static string GetAssetsPath(string localPath) => Path.Combine("Assets", localPath);
        private static string GetExportedPackageFileName(PackageName enumName) => $"MustHave{enumName}.unitypackage";

#if MUSTHAVE_BUILD_POSTPROCESS
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

                FileUtils.TryCopyFile(srcMustHaveLibPath, dstMustHaveLibPath);
            }
            var assemblyFolderPath = Path.Combine(ProjectFolderPath, "Library", "ScriptAssemblies");
            var mustHaveAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveLibName);
            var mustHaveEditorAssemblyLibPath = Path.Combine(assemblyFolderPath, MustHaveEditorLibName);

            FileUtils.TryCopyFile(mustHaveAssemblyLibPath, GetFullPath(MustHaveEditorLibPath));
            FileUtils.TryCopyFile(mustHaveEditorAssemblyLibPath, GetFullPath(MustHaveEditorEditorLibPath));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        //[MenuItem("Tools/Move MustHave/Shared to hidden")]
        public static void MoveSharedFolderToHiddenFolder(bool refresh)
        {
            FileUtils.TryMoveFile(GetFullPath(SharedFolderMetaPath), GetFullPath(SharedHiddenFolderMetaPath));
            FileUtils.TryMoveDirectory(GetFullPath(SharedFolderPath), GetFullPath(SharedHiddenFolderPath));

            if (refresh)
            {
                AssetUtils.SaveAndRefresh();
            }
        }

        //[MenuItem("Tools/Move MustHave/Shared from hidden")]
        public static void MoveSharedFolderFromHiddenFolder(bool refresh)
        {
            FileUtils.TryMoveFile(GetFullPath(SharedHiddenFolderMetaPath), GetFullPath(SharedFolderMetaPath));
            FileUtils.TryMoveDirectory(GetFullPath(SharedHiddenFolderPath), GetFullPath(SharedFolderPath));

            if (refresh)
            {
                AssetUtils.SaveAndRefresh();
            }
        }

        public static void EnableMustHaveLibsPlatforms()
        {
            MoveSharedFolderToHiddenFolder(false);

            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveStandaloneLibPath)) as PluginImporter;
            if (importer)
            {
                SetCompatibleWithAnyPlatformExceptEditor(importer);
                importer.SaveAndReimport();
            }
            SetMustHaveEditorLibsCompatibleWithEditor(true);

            AssetUtils.SaveAndRefresh();
        }

        public static void DisableMustHaveLibsPlatforms()
        {
            MoveSharedFolderFromHiddenFolder(false);

            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveStandaloneLibPath)) as PluginImporter;
            if (importer)
            {
                SetNonCompatibleWithAnyPlatform(importer);
                importer.SaveAndReimport();
            }
            SetMustHaveEditorLibsCompatibleWithEditor(false);

            AssetUtils.SaveAndRefresh();
        }

        public static void ExportMustHavePackage(PackageName enumName)
        {
            //TODO: Remove UniversalAdditionalCameraData from OutlineObjectCamera prefab before export

            var assetPaths = new string[] {
                $"Assets/Packages/MustHave/{enumName}",
                "Assets/Packages/MustHave/SharedScripts",
                "Assets/Packages/MustHave/SharedPlugins"
            };
            var packageFolderName = ExportedPackageFolderName;
            var packageFolderPath = Path.Combine(ProjectFolderPath, packageFolderName);
            var packageFileName = GetExportedPackageFileName(enumName);
            var packageFileLocalPath = Path.Combine(packageFolderName, packageFileName);

            if (!Directory.Exists(packageFolderPath))
            {
                Directory.CreateDirectory(packageFolderPath);
            }
            AssetDatabase.ExportPackage(assetPaths, packageFileLocalPath, ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
            AssetUtils.SaveAndRefresh();
        }

        private static void SetMustHaveEditorLibsCompatibleWithEditor(bool compatible)
        {
            var importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveEditorLibPath)) as PluginImporter;
            if (importer)
            {
                SetCompatibleWithEditorOnly(importer, compatible);
                importer.SaveAndReimport();
            }
            importer = AssetImporter.GetAtPath(GetAssetsPath(MustHaveEditorEditorLibPath)) as PluginImporter;
            if (importer)
            {
                SetCompatibleWithEditorOnly(importer, compatible);
                importer.SaveAndReimport();
            }
        }

        private static void SetCompatibleWithEditorOnly(PluginImporter importer, bool compatible)
        {
            SetNonCompatibleWithAnyPlatform(importer);
            importer.SetCompatibleWithEditor(compatible);
        }

        private static void SetCompatibleWithAnyPlatformExceptEditor(PluginImporter importer)
        {
            importer.SetCompatibleWithAnyPlatform(true);
            importer.SetExcludeEditorFromAnyPlatform(true);
            SetExcludeStandaloneFromAnyPlatform(importer, false);
        }

        private static void SetNonCompatibleWithAnyPlatform(PluginImporter importer)
        {
            importer.SetCompatibleWithEditor(false);
            SetCompatibleWithStandalonePlatforms(importer, false);
            importer.SetCompatibleWithAnyPlatform(false);
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
    }
}
