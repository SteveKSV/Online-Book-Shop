using Catalog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreManager _manager;

        public GenreController(IGenreManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> GetAllGenres()
        {
            var genres = await _manager.GetAllGenres();
            return Ok(genres);
        }
    }
}
