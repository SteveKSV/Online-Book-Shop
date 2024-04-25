using Client.Models;
using Client.Pages;
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Client.Shared
{
    public partial class Pagination
    {
        [Parameter]
        public List<BookModel> Books { get; set; }
        [Parameter] public int TotalPages { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private ITokenService TokenService { get; set; }
        [Parameter] public EventCallback<string> OnLoadBooks { get; set; }
        private int CurPage = 1;

        protected bool CanGoPreviousPage => CurPage > 1;
        protected bool CanGoNextPage => CurPage < TotalPages;

        protected override async Task OnInitializedAsync()
        {
            await ShowPage();
        }

        protected async Task NextPage()
        {
            if (CanGoNextPage)
            {
                CurPage++;
                await ShowPage();
            }
        }

        protected async Task PrevPage()
        {
            if (CanGoPreviousPage)
            {
                CurPage--;
                await ShowPage();
            }
        }

        protected async Task ShowPage(int i)
        {
            CurPage = i;
            await ShowPage();
        }

        protected async Task ShowPage()
        {
            await OnLoadBooks.InvokeAsync($"?paginationParams.pageNumber={CurPage}&paginationParams.pageSize=3");
        }
    }
}
