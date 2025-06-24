using Catalog.DTO;

namespace Catalog.Managers.Interfaces
{
    public interface ICommentManager
    {
        Task<CommentDTO?> GetCommentByIdAsync(Guid commentId);
        Task<CommentDTO> AddCommentAsync(AddCommentDTO newCommentDto);
        Task<CommentDTO?> UpdateCommentAsync(AddCommentDTO updatedCommentDto);
        Task<bool> DeleteCommentAsync(Guid commentId);
    }
}
