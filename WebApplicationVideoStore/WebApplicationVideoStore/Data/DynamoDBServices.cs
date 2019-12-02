using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Global.Models;

namespace Global.Data
{
    public class DynamoDBServices
    {
        IAmazonDynamoDB dynamoDBClient { get; set; }

        public DynamoDBServices(IAmazonDynamoDB dynamoDBClient)
        {
            this.dynamoDBClient = dynamoDBClient;
        }

        public async Task<Book> InsertBook(Book book)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            // Add a unique id for the primary key.
            book.Id = System.Guid.NewGuid().ToString();
            await context.SaveAsync(book, default(System.Threading.CancellationToken));
            Book newBook = await context.LoadAsync<Book>(book.Id, default(System.Threading.CancellationToken));
            return book;
        }

        public async Task<Book> GetBookAsync(string Id)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            Book newBook = await context.LoadAsync<Book>(Id, default(System.Threading.CancellationToken));
            return newBook;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.SaveAsync(book, default(System.Threading.CancellationToken));
            Book newBook = await context.LoadAsync<Book>(book.Id, default(System.Threading.CancellationToken));
            return newBook;
        }
        public async Task DeleteBookAsync(string Id)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.DeleteAsync(Id, default(System.Threading.CancellationToken));
        }
        public async Task<List<Book>> GetBooksAsync()
        {
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("Id", ScanOperator.NotEqual, 0);

            ScanOperationConfig soc = new ScanOperationConfig()
            {
                // AttributesToGet = new List { "Id", "Title", "ISBN", "Price" },
                Filter = scanFilter
            };
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            AsyncSearch<Book> search = context.FromScanAsync<Book>(soc, null);
            List<Book> documentList = new List<Book>();
            do
            {
                documentList = await search.GetNextSetAsync(default(System.Threading.CancellationToken));
            } while (!search.IsDone);

            return documentList;
        }
    }
}