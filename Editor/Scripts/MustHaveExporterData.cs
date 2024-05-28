using UnityEditor;
using UnityEngine;

namespace MustHave
{
    public enum PackageName
    {
        IconsRenderer,
        Outline
    }

    [CreateAssetMenu(fileName = "MustHaveExporterData", menuName = "MustHave/ScriptableObjects/MustHaveExporterData")]
    public class MustHaveExporterData : ScriptableObject
    {
        public PackageName PackageName { get => packageName; set => packageName = value; }
        public BuildTarget BuildTarget { get => buildTarget; set => buildTarget = value; }

        [SerializeField]
        private PackageName packageName = PackageName.Outline;

        [SerializeField]
        private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    }
}
