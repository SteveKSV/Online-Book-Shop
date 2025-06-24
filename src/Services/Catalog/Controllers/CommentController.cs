using Catalog.DTO;
using Catalog.Entities;
using Catalog.Managers;
using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentManager _manager;

        public CommentController(ICommentManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Add a new comment to a book.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CommentDTO>> AddComment([FromBody] AddCommentDTO commentDto)
        {
            var result = await _manager.AddCommentAsync(commentDto);
            return CreatedAtAction(nameof(GetComment), new { id = result.Id }, result);
        }

        /// <summary>
        /// Get a specific comment by ID (optional helper for CreatedAtAction).
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetComment(Guid id)
        {
            var comment = await _manager.GetCommentByIdAsync(id);
            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        /// <summary>
        /// Update an existing comment.
        /// </summary>
        [HttpPut("update-comment")]
        public async Task<ActionResult<CommentDTO>> UpdateComment([FromBody] AddCommentDTO commentDto)
        {
            var result = await _manager.UpdateCommentAsync(commentDto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Delete a comment by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _manager.DeleteCommentAsync(id);
            if (!result)
                return NotFound();

            return Ok(result);
        }
    }
}
