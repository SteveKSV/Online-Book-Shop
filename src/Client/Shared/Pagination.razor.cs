using Microsoft.AspNetCore.Components;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Shared
{
    public partial class Pagination
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [Parameter] public PaginationMetadata PaginationInfo { get; set; } = default!;
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }

        protected List<int> DisplayPages { get; private set; } = new();

        protected override void OnParametersSet()
        {
            CalculateDisplayPages();
        }

        private void CalculateDisplayPages()
        {
            DisplayPages.Clear();

            int startPage = Math.Max(1, PaginationInfo.CurrentPage - 1);
            int endPage = Math.Min(PaginationInfo.TotalPages, PaginationInfo.CurrentPage + 1);

            for (int i = startPage; i <= endPage; i++)
            {
                DisplayPages.Add(i);
            }
        }

        private async Task NavigateToPage(int page)
        {
            if (page != PaginationInfo.CurrentPage)
            {
                await OnPageChanged.InvokeAsync(page);
            }
        }
    }
}
