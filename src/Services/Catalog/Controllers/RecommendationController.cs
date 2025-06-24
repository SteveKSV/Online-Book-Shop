using Catalog.DTO;
using Catalog.Managers;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationManager _manager;

        public RecommendationController(IRecommendationManager manager)
        {
            _manager = manager;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(List<RecommendedBookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRecommendations(Guid userId)
        {
            var result = await _manager.GetRecommendationsAsync(userId);
            return Ok(result);
        }

        [HttpGet("top")]
        [ProducesResponseType(typeof(List<RecommendedBookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<RecommendedBookDto>>> GetTopBooks()
        {
            try
            {
                var topBooks = await _manager.GetTopBooksAsync();
                return Ok(topBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while fetching top books: {ex.Message}");
            }
        }

        [HttpGet("new")]
        [ProducesResponseType(typeof(List<RecommendedBookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNewBooks()
        {
            var books = await _manager.GetNewBooksAsync();
            return Ok(books);
        }
    }

}
