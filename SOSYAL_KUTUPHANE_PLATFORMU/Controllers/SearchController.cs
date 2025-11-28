using Microsoft.AspNetCore.Mvc;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Services;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;
using System.ComponentModel.DataAnnotations;

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

  /// <summary>
 /// Film ara (TMDb API)
        /// GET: api/search/movies?query=inception&pageNumber=1&pageSize=20
        /// </summary>
        [HttpGet("movies")]
        public async Task<ActionResult<ApiResponse<PagedResult<SearchResultItemDto>>>> SearchMovies(
  [FromQuery, Required(ErrorMessage = "Arama sorgusu zorunludur")] string query,
      [FromQuery] int pageNumber = 1,
 [FromQuery] int pageSize = 20)
      {
          if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(ApiResponse<PagedResult<SearchResultItemDto>>.FailResponse(
            "Arama sorgusu boş olamaz."));
            }

            if (pageSize <= 0 || pageSize > 50) pageSize = 20;
      if (pageNumber <= 0) pageNumber = 1;

     var allResults = await _tmdbService.SearchMoviesAsync(query);
var totalCount = allResults.Count;

        var pagedResults = allResults
 .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
   .ToList();

            var pagedResult = new PagedResult<SearchResultItemDto>(
              pagedResults, totalCount, pageNumber, pageSize);

        return Ok(ApiResponse<PagedResult<SearchResultItemDto>>.SuccessResponse(
       pagedResult, $"'{query}' için {totalCount} film bulundu."));
        }

        /// <summary>
   /// Kitap ara (Google Books API)
        /// GET: api/search/books?query=harry+potter&pageNumber=1&pageSize=20
      /// </summary>
        [HttpGet("books")]
        public async Task<ActionResult<ApiResponse<PagedResult<SearchResultItemDto>>>> SearchBooks(
    [FromQuery, Required(ErrorMessage = "Arama sorgusu zorunludur")] string query,
        [FromQuery] int pageNumber = 1,
 [FromQuery] int pageSize = 20)
        {
   if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(ApiResponse<PagedResult<SearchResultItemDto>>.FailResponse(
          "Arama sorgusu boş olamaz."));
     }

     if (pageSize <= 0 || pageSize > 50) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

       var allResults = await _googleBooksService.SearchBooksAsync(query);
            var totalCount = allResults.Count;

     var pagedResults = allResults
   .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
         .ToList();

    var pagedResult = new PagedResult<SearchResultItemDto>(
      pagedResults, totalCount, pageNumber, pageSize);

 return Ok(ApiResponse<PagedResult<SearchResultItemDto>>.SuccessResponse(
           pagedResult, $"'{query}' için {totalCount} kitap bulundu."));
        }

        /// <summary>
        /// Tüm içeriklerde ara (Film + Kitap birleşik)
      /// GET: api/search/all?query=...&pageNumber=1&pageSize=20
        /// </summary>
        [HttpGet("all")]
      public async Task<ActionResult<ApiResponse<PagedResult<SearchResultItemDto>>>> SearchAll(
            [FromQuery, Required(ErrorMessage = "Arama sorgusu zorunludur")] string query,
            [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
   {
          if (string.IsNullOrWhiteSpace(query))
            {
     return BadRequest(ApiResponse<PagedResult<SearchResultItemDto>>.FailResponse(
        "Arama sorgusu boş olamaz."));
            }

     if (pageSize <= 0 || pageSize > 50) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

  // Her iki API'den de sonuç getir
 var moviesTask = _tmdbService.SearchMoviesAsync(query);
    var booksTask = _googleBooksService.SearchBooksAsync(query);

 await Task.WhenAll(moviesTask, booksTask);

     var allResults = new List<SearchResultItemDto>();
  allResults.AddRange(moviesTask.Result);
            allResults.AddRange(booksTask.Result);

       var totalCount = allResults.Count;

            var pagedResults = allResults
                .Skip((pageNumber - 1) * pageSize)
   .Take(pageSize)
        .ToList();

          var pagedResult = new PagedResult<SearchResultItemDto>(
        pagedResults, totalCount, pageNumber, pageSize);

  return Ok(ApiResponse<PagedResult<SearchResultItemDto>>.SuccessResponse(
           pagedResult, $"'{query}' için toplam {totalCount} sonuç bulundu."));
        }
    }
}
