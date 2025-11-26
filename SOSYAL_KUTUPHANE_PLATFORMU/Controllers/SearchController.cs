using Microsoft.AspNetCore.Mvc;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Services;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ITmdbService _tmdbService;
        private readonly IGoogleBooksService _googleBooksService;

        public SearchController(ITmdbService tmdbService, IGoogleBooksService googleBooksService)
        {
            _tmdbService = tmdbService;
            _googleBooksService = googleBooksService;
        }

        // GET: api/search/movies?query=...
        [HttpGet("movies")]
        public async Task<ActionResult<List<SearchResultItemDto>>> SearchMovies([FromQuery] string query)
        {
            var results = await _tmdbService.SearchMoviesAsync(query);
            return Ok(results);
        }

        // GET: api/search/books?query=...
        [HttpGet("books")]
        public async Task<ActionResult<List<SearchResultItemDto>>> SearchBooks([FromQuery] string query)
        {
            var results = await _googleBooksService.SearchBooksAsync(query);
            return Ok(results);
        }
    }
}
