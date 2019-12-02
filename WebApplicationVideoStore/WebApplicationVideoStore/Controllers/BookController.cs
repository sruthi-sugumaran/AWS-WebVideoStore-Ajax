using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Global.Models;
using Global.Data;
using Amazon.DynamoDBv2;

namespace Global.Controllers
{
    public class BooksController : Controller
    {
        private IAmazonDynamoDB dynamoDBClient;


        public BooksController(IAmazonDynamoDB dynamoDBClient)
        {
            //_context = context;
            this.dynamoDBClient = dynamoDBClient;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
            
            return View(await service.GetBooksAsync());
           
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
            Book book = await service.GetBookAsync(id);
            
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ISBN,InPublication,PageCount,Price,ProductCategory,Title")] Book book)
        {
            if (ModelState.IsValid)
            {
                DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
                Book newBook = await service.InsertBook(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
            Book book = await service.GetBookAsync(id);

            //var book = await _context.Book.SingleOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ISBN,InPublication,PageCount,Price,ProductCategory,Title")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
                    Book newBook = await service.UpdateBookAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
            Book book = await service.GetBookAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            DynamoDBServices service = new DynamoDBServices(dynamoDBClient);
            await service.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}