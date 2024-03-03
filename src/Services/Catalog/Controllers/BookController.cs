using Catalog.Entities;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookManager _manager;

        public BookController(IBookManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var products = await _manager.GetBooks();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetBookById")]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Book>> GetBookById(string id)
        {
            var book = await _manager.GetBookById(id);

            if (book != null)
            {
                return Ok(book);
            }

            return NotFound();
        }

        [HttpGet("GetBookByTitle/{title}")]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Book>> GetBookByTitle(string title)
        {
            var book = await _manager.GetBookByTitle(title);
            return Ok(book);
        }

        [HttpGet("GetBooksByAuthor/{authorName}")]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Book>> GetBooksByAuthor(string authorName)
        {
            var books = await _manager.GetBooksByAuthor(authorName);
            return Ok(books);
        }

        [HttpGet("GetBooksByPublisher/{publisher}")]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Book>> GetBooksByPublisher(string publisher)
        {
            var books = await _manager.GetBooksByPublisher(publisher);
            return Ok(books);
        }

        [HttpGet("GetBooksByGenre/{genre}")]
        [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Book>> GetBooksByGenre(string genre)
        {
            var books = await _manager.GetBooksByGenre(genre);
            return Ok(books);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            // Generate a random 24-digit hexadecimal string for the Id
            book.Id = GenerateRandomHexadecimalId();

            await _manager.CreateEntity(book);

            // Issue may be here: Check the 'new { id = book.Id }' part
            return CreatedAtRoute("GetBookById", new { id = book.Id }, book);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            return Ok(await _manager.UpdateEntity(book));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBookById(string id)
        {
            return Ok(await _manager.DeleteEntity(id));
        }
        private string GenerateRandomHexadecimalId()
        {
            // Generate a random 24-digit hexadecimal string
            Random random = new Random();
            byte[] buffer = new byte[12];
            random.NextBytes(buffer);
            string randomHexId = string.Concat(buffer.Select(b => b.ToString("x2")));
            return randomHexId;
        }
    }
}