using System.Collections.Generic;

namespace MustHave
{
    public class BucketRandomizer : ListRandomizer<bool>
    {
        private int totalCount = -1;
        private int successCount = -1;

        public BucketRandomizer()
        {
        }

        public void SetParameters(int totalCount, int successCount)
        {
            this.totalCount = totalCount;
            this.successCount = successCount;

            SetList(CreateBucket());
        }

        public void SetParameters(BucketRandomizerParameters parameters)
        {
            SetParameters(parameters.TotalCount, parameters.SuccessCount);
        }

        private List<bool> CreateBucket()
        {
            List<bool> bucket = new List<bool>();
            for (int i = 0; i < totalCount; i++)
            {
                bucket.Add(i < successCount);
            }
            return bucket;
        }
    } 
}