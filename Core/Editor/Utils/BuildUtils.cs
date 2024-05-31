using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

namespace MustHave
{
    public static class BuildUtils
    {
        public static readonly string ProjectFolderPath = Directory.GetParent(Application.dataPath).FullName;

        public static string GetPlatformExecExtension(BuildTarget buildTarget) => buildTarget switch
        {
            //BuildTarget.StandaloneOSX => ".app",
            BuildTarget.StandaloneOSX => ".dmg",
            BuildTarget.StandaloneWindows => ".exe",
            BuildTarget.iOS => ".ipa",
            BuildTarget.StandaloneWindows64 => ".exe",
            BuildTarget.WebGL => string.Empty,
            BuildTarget.WSAPlayer => string.Empty,
            BuildTarget.StandaloneLinux64 => ".x64",
            BuildTarget.Android => ".apk",
            BuildTarget.PS4 => string.Empty,
            BuildTarget.XboxOne => string.Empty,
            BuildTarget.tvOS => string.Empty,
            BuildTarget.Switch => string.Empty,
            BuildTarget.Lumin => string.Empty,
            BuildTarget.Stadia => string.Empty,
            BuildTarget.GameCoreXboxOne => string.Empty,
            BuildTarget.PS5 => string.Empty,
            BuildTarget.EmbeddedLinux => string.Empty,
            BuildTarget.NoTarget => string.Empty,
            BuildTarget.LinuxHeadlessSimulation => string.Empty,
            BuildTarget.GameCoreXboxSeries => string.Empty,
            //BuildTarget.StandaloneOSXUniversal => throw new NotImplementedException(),
            //BuildTarget.StandaloneOSXIntel => throw new NotImplementedException(),
            //BuildTarget.WebPlayer => throw new NotImplementedException(),
            //BuildTarget.WebPlayerStreamed => throw new NotImplementedException(),
            //BuildTarget.PS3 => throw new NotImplementedException(),
            //BuildTarget.XBOX360 => throw new NotImplementedException(),
            //BuildTarget.StandaloneLinux => throw new NotImplementedException(),
            //BuildTarget.StandaloneLinuxUniversal => throw new NotImplementedException(),
            //BuildTarget.WP8Player => throw new NotImplementedException(),
            //BuildTarget.StandaloneOSXIntel64 => throw new NotImplementedException(),
            //BuildTarget.BlackBerry => throw new NotImplementedException(),
            //BuildTarget.Tizen => throw new NotImplementedException(),
            //BuildTarget.PSP2 => throw new NotImplementedException(),
            //BuildTarget.PSM => throw new NotImplementedException(),
            //BuildTarget.SamsungTV => throw new NotImplementedException(),
            //BuildTarget.N3DS => throw new NotImplementedException(),
            //BuildTarget.WiiU => throw new NotImplementedException(),
            //BuildTarget.iPhone => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        public static void BuildActiveScene(BuildTarget buildTarget)
        {
            var buildsFolderName = "Builds";
            var buildsFolderFullPath = Path.Combine(ProjectFolderPath, buildsFolderName);
            var buildExecExtension = GetPlatformExecExtension(buildTarget);
            var buildLocalPath = Path.Combine(buildsFolderName, $"{Application.productName}{buildExecExtension}");

            if (!Directory.Exists(buildsFolderFullPath))
            {
                Directory.CreateDirectory(buildsFolderFullPath);
            }
            var activeScenePath = SceneManager.GetActiveScene().path;
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { activeScenePath },
                locationPathName = buildLocalPath,
                target = buildTarget,
                options = BuildOptions.None
            };
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
    }
}
