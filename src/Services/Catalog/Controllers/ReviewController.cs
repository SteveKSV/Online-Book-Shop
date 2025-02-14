using Catalog.Entities;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Net;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewManager _manager;

        public ReviewController(IReviewManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Review>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews([FromQuery] PaginationParams? paginationParams = null)
        {
            var reviews = await _manager.GetAllReviewsAsync(paginationParams);
            return Ok(reviews);
        }

        // Get review by ID
       [HttpGet("{id}", Name = "GetReviewById")]
       [ProducesResponseType(typeof(Review), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Review>> GetReviewById(string id)
        {
            var review = await _manager.GetReviewById(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // Get reviews by book ID
        [HttpGet("byBook/{bookId}")]
        [ProducesResponseType(typeof(IEnumerable<Review>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByBookId(string bookId)
        {
            var reviews = await _manager.GetAllReviewsByBookId(bookId);
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Review), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            // Validate UserId as a UUID
            if (!Guid.TryParse(review.UserId.ToString(), out _))
            {
                return BadRequest("Invalid UserId. Must be a valid UUID.");
            }

            // Generate a valid MongoDB ObjectId
            review.Id = ObjectId.GenerateNewId().ToString();

            await _manager.CreateEntity(review);

            return CreatedAtRoute("GetReviewById", new { id = review.Id }, review);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Review), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateReview([FromBody] Review review)
        {
            return Ok(await _manager.UpdateEntity(review));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Review), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteReviewById(string id)
        {
            return Ok(await _manager.DeleteEntity(id));
        }

        private static string GenerateRandomHexadecimalId()
        {
            // Generate a random 24-digit hexadecimal string
            var random = new Random();
            var buffer = new byte[12];
            random.NextBytes(buffer);
            var randomHexId = string.Concat(buffer.Select(b => b.ToString("x2")));
            return randomHexId;
        }
    }
}
