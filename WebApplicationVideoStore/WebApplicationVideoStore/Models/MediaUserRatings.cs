using Amazon.DynamoDBv2.DataModel;

namespace WebApplicationVideoStore.Models
{
    [DynamoDBTable("mediauserratings")]
    public class MediaUserRatings
    {
        [DynamoDBHashKey]
        public string GeneratedFileNameAsMediaId { get; set; }
        [DynamoDBRangeKey]
        public string RatedUserEmailId { get; set; }
        [DynamoDBProperty]
        public int RatingValue { get; set; }

        public MediaUserRatings() { }
        public MediaUserRatings(string GeneratedFileNameAsMediaId, string RatedUserEmailId, int RatingValue) {
            this.GeneratedFileNameAsMediaId = GeneratedFileNameAsMediaId;
            this.RatedUserEmailId = RatedUserEmailId;
            this.RatingValue = RatingValue;
        }
    }
}
