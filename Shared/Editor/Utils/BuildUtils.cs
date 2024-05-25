using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace MustHave
{
    public static class BuildUtils
    {
        public static readonly string ProjectFolderPath = Directory.GetParent(Application.dataPath).FullName;

        public static void BuildActiveScene(BuildTarget buildTarget, string extension)
        {
            var buildsFolderName = "Builds";
            var buildsFolderFullPath = Path.Combine(ProjectFolderPath, buildsFolderName);
            var buildLocalPath = Path.Combine(buildsFolderName, $"{Application.productName}.{extension}");

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
