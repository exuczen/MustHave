using UnityEngine;

namespace MustHave
{
    [CreateAssetMenu(menuName = "Dialogue System/BucketRandomizerParameters")]
    public class BucketRandomizerParameters : ScriptableObject
    {
        public int SuccessCount => successCount;
        public int TotalCount => totalCount;

        [SerializeField]
        private int successCount = 3;
        [SerializeField]
        private int totalCount = 3;
    }
}