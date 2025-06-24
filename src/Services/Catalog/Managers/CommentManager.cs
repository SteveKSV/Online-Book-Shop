using AutoMapper;
using Catalog.DTO;
using Catalog.Entities;
using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Managers
{
    public class CommentManager : ICommentManager
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentManager(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommentDTO?> GetCommentByIdAsync(Guid commentId)
        {
            try
            {
                var comment = await _context.Comments.Include(c => c.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                return _mapper.Map<CommentDTO?>(comment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get comment by id '{commentId}'.", ex);
            }
        }
        public async Task<CommentDTO> AddCommentAsync(AddCommentDTO newCommentDto)
        {
            try
            {
                var comment = _mapper.Map<Comment>(newCommentDto);
                comment.Id = Guid.NewGuid();
                comment.CommentedAt = DateTime.UtcNow;

                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add comment.", ex);
            }
        }

        public async Task<CommentDTO?> UpdateCommentAsync(AddCommentDTO updatedCommentDto)
        {
            try
            {
                var existingComment = await _context.Comments.FindAsync(updatedCommentDto.Id);

                if (existingComment == null)
                    return null;

                existingComment.CommentText = updatedCommentDto.CommentText;
                existingComment.CommentedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return _mapper.Map<CommentDTO>(existingComment);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update comment.", ex);
            }
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null)
                    return false;

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete comment with id '{commentId}'.", ex);
            }
        }
    }
}
