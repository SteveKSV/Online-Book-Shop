using Catalog.DTO;
using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [ProducesResponseType(typeof(PagedList<BookDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PagedList<BookDTO>>> GetBooks(
            [FromQuery] PaginationParams? paginationParams = null, string? title = null, string? sortOrder = null,
            [FromQuery] string? genre = null, [FromQuery] string? sortRating = null
            )
        {
            try
            {
                var products = await _manager.GetBooks(paginationParams, title, sortOrder, genre, sortRating);
                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.TotalPages,
                    products.HasNext,
                    products.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while fetching books: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BookDTO>> GetBookById(Guid id)
        {
            try
            {
                var book = await _manager.GetBookById(id);

                if (book == null)
                    return NotFound($"Book with id '{id}' not found.");

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while fetching book by id: {ex.Message}");
            }
        }

        [HttpGet("GetBookByTitle/{title}")]
        [ProducesResponseType(typeof(BookDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BookDTO>> GetBookByTitle(string title)
        {
            try
            {
                var book = await _manager.GetBookByTitle(title);

                if (book == null)
                    return NotFound($"Book with title '{title}' not found.");

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while fetching book by title: {ex.Message}");
            }
        }

        [HttpGet("{bookId}/comments")]
        [ProducesResponseType(typeof(PagedList<CommentDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PagedList<CommentDTO>>> GetCommentsForBook(Guid bookId, [FromQuery] PaginationParams paginationParams)
        {
            try
            {
                var commentsPaged = await _manager.GetCommentsForBook(bookId, paginationParams);

                var metadata = new
                {
                    commentsPaged.TotalCount,
                    commentsPaged.PageSize,
                    commentsPaged.CurrentPage,
                    commentsPaged.TotalPages,
                    commentsPaged.HasNext,
                    commentsPaged.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(commentsPaged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while fetching comments: {ex.Message}");
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(BookDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] BookCreateDTO bookCreateDto)
        {
            try
            {
                var createdBook = await _manager.CreateBookAsync(bookCreateDto);

                // Тут повертаємо Id створеної книги, щоб CreatedAtRoute працював коректно
                return CreatedAtRoute("GetBookById", new { id = createdBook.Id }, createdBook);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while creating book: {ex.Message}");
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(BookDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateBook([FromBody] BookCreateDTO bookUpdateDto)
        {
            try
            {
                var updatedBook = await _manager.UpdateBookAsync(bookUpdateDto);

                if (updatedBook == null)
                    return NotFound($"Book with id '{bookUpdateDto.Id}' not found.");

                return Ok(updatedBook);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while updating book: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBookById(Guid id)
        {
            try
            {
                var deleted = await _manager.DeleteEntity(id);

                if (!deleted)
                    return NotFound($"Book with id '{id}' not found.");

                return Ok($"Book with id '{id}' deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while deleting book: {ex.Message}");
            }
        }
    }
}
