using Client.Models.Catalog;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Shared
{
    public partial class CommentsComponent : ComponentBase
    {
        [Parameter]
        public Guid BookId { get; set; }

        [Parameter]
        public bool IsLoggedIn { get; set; } = false;

        [Inject]
        public ICatalogService CatalogService { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        private List<Comment> Comments = new();
        private Dictionary<Guid, bool> expandedStates = new();
        private const int MaxLength = 150;

        private PaginationMetadata PaginationInfo = new();
        private bool IsLoading = true;
        private int PageSize = 5;

        private Guid CurrentUserId;
        private bool IsAdmin;

        private bool IsEditing = false;
        private AddUpdateComment? EditingComment;

        private string NewCommentText = "";

        private async Task AddComment()
        {
            if (!string.IsNullOrWhiteSpace(NewCommentText))
            {
                var newComment = new AddUpdateComment
                {
                    UserId = CurrentUserId,
                    BookId = BookId,
                    CommentText = NewCommentText,
                    CommentedAt = DateTime.UtcNow
                };

                var success = await CatalogService.AddCommentAsync(newComment);

                if (success)
                {
                    NewCommentText = "";
                    await LoadComments(1);
                }
                else
                {
                    // Покажіть помилку
                }
            }
        }

        private void StartEditing(Comment comment)
        {
            IsEditing = true;
            EditingComment = new AddUpdateComment
            {
                Id = comment.Id,
                UserId = comment.UserId,
                BookId = BookId,
                CommentText = comment.CommentText,
                CommentedAt = comment.CommentedAt
            };
        }

        private async Task SaveEditedComment()
        {
            if (EditingComment != null)
            {
                var success = await CatalogService.UpdateCommentAsync(EditingComment);

                if (success)
                {
                    IsEditing = false;
                    EditingComment = null;
                    await LoadComments(1);
                }
                else
                {
                    // Помилка редагування
                }
            }
        }

        private void CancelEditing()
        {
            IsEditing = false;
            EditingComment = null;
        }
        private async Task DeleteComment(Guid commentId)
        {
            var success = await CatalogService.DeleteCommentAsync(commentId);

            if (success)
            {
                await LoadComments(1);
            }
            else
            {
                // Помилка видалення
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.FindFirst("sub")?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                    CurrentUserId = userId;

                IsAdmin = user.IsInRole("Admin");
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            await LoadComments(1);
        }

        private async Task LoadComments(int pageNumber)
        {
            IsLoading = true;
            StateHasChanged();

            string queryString = $"pageNumber={pageNumber}&pageSize={PageSize}";
            var (loadedComments, pagination) = await CatalogService.GetCommentsForBook(BookId, queryString);

            Comments = loadedComments;
            PaginationInfo = pagination;

            expandedStates.Clear();
            IsLoading = false;
            StateHasChanged();
        }

        private void ToggleExpanded(Guid commentId)
        {
            if (expandedStates.ContainsKey(commentId))
                expandedStates[commentId] = !expandedStates[commentId];
            else
                expandedStates[commentId] = true;
        }

        private string GetShortText(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= MaxLength)
                return text;

            int lastSpace = text.LastIndexOf(' ', MaxLength);
            if (lastSpace == -1) lastSpace = MaxLength;

            return text.Substring(0, lastSpace) + "...";
        }
    }
}
