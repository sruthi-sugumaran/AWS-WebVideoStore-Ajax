using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DataModel;

namespace Global.Models
{
    [DynamoDBTable("ProductTable")]
    public class Book
    {
        [DynamoDBHashKey]    //Partition key
        public string Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public double Price { get; set; }
        public string PageCount { get; set; }
        public string ProductCategory { get; set; }
        public bool InPublication { get; set; }

    }
}
