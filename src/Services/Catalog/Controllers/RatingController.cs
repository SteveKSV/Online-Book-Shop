using Catalog.DTO;
using Catalog.Entities;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingManager _ratingManager;

        public RatingController(IRatingManager ratingManager)
        {
            _ratingManager = ratingManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetRatings([FromQuery] PaginationParams? paginationParams = null)
        {
            try
            {
                var ratings = await _ratingManager.GetAllRatings(paginationParams!.PageNumber, paginationParams.PageSize);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ratings.", error = ex.Message });
            }
        }

        [HttpGet("{bookId:guid}/{userId:guid}")]
        public async Task<ActionResult<int>> GetUserRating(Guid bookId, Guid userId)
        {
            try
            {
                var rating = await _ratingManager.GetUserRating(bookId, userId);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ratings.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Rate([FromBody] RateBookDTO request)
        {
            try
            {
                var result = await _ratingManager.RateAsync(request.UserId, request.BookId, request.Rating);
                return Ok(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rating the book.", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRatingDTO request)
        {
            try
            {
                var updatedRating = await _ratingManager.UpdateRatingAsync(request.UserId, request.BookId, request.NewRating);
                return Ok(updatedRating);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the rating.", error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid userId, [FromQuery] Guid bookId)
        {
            try
            {
                var deleted = await _ratingManager.DeleteRatingAsync(userId, bookId);
                if (!deleted)
                    return NotFound(new { message = "Rating not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the rating.", error = ex.Message });
            }
        }
    }
}
