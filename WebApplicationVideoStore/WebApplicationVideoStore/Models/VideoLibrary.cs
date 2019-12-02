using System;
using System.Text.RegularExpressions;
using Amazon.DynamoDBv2.DataModel;

namespace WebApplicationVideoStore.Models
{

    [DynamoDBTable("lab3videos")]
    public class VideoLibrary
    {
        [DynamoDBHashKey]
        public string OwnerEmailId { get; set; }
        [DynamoDBRangeKey]
        public string FileName { get; set; }
        [DynamoDBProperty]
        public string GeneratedFileNameAsMediaId { get; set; }
        [DynamoDBProperty]
        public string UploadedTime { get; set; }
        [DynamoDBProperty]
        public int Likes { get; set; }
        [DynamoDBProperty]
        public int Downloads { get; set; }

        public VideoLibrary() { }

        public VideoLibrary(string OwnerEmailId, string FileName)
        {
            this.OwnerEmailId = OwnerEmailId;
            this.FileName = FileName;
            UploadedTime = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            GeneratedFileNameAsMediaId = (DateTime.Now.ToString("yyyyMMddHHmmss") + "_" +
                Regex.Replace(this.OwnerEmailId, @"\p{P}", "") + "_" + FileName.ToLowerInvariant().Replace(" ", string.Empty));
            Likes = 0;
            Downloads = 0;
        }
    }
}
