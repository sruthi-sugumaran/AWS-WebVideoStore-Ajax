using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplicationVideoStore.Models
{
    [DynamoDBTable("lab3mediacomments")]
    public class MediaComments
    {
        [DynamoDBHashKey]
        public string GeneratedFileNameAsMediaId { get; set; }
        [DynamoDBRangeKey]
        public string CommentId { get; set; }
        [DynamoDBProperty]
        public string CommentedUserEmailId { get; set; }
        [DynamoDBProperty]
        public string CommentText { get; set; }
        [DynamoDBProperty]
        public string CommentCreationTime { get; set; }

        public MediaComments() { }
        public MediaComments(string GeneratedFileNameAsMediaId, string CommentedUserEmailId, string CommentText)
        {
            this.GeneratedFileNameAsMediaId = GeneratedFileNameAsMediaId;
            this.CommentedUserEmailId = CommentedUserEmailId;
            this.CommentText = CommentText;
            this.CommentCreationTime = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            CommentId = this.GeneratedFileNameAsMediaId 
                + Regex.Replace(this.CommentedUserEmailId.ToLower(), @"\p{P}", "") 
                + DateTime.Now.ToString("M d h:mm yy").ToLowerInvariant().Replace(" ", string.Empty).Replace(",",string.Empty);
        }
    }
}
