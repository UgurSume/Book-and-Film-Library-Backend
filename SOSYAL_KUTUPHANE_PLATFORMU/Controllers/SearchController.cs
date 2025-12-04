using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
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
        private readonly ApplicationDbContext _context;
      private readonly ILogger<SearchController> _logger;

    public SearchController(
          ITmdbService tmdbService, 
         IGoogleBooksService googleBooksService,
   ApplicationDbContext context,
   ILogger<SearchController> logger)
   {
   _tmdbService = tmdbService;
_googleBooksService = googleBooksService;
          _context = context;
            _logger = logger;
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

  /// <summary>
        /// Detaylı içerik filtreleme - Platform içindeki içeriklerde arama
        /// POST: api/search/filter
        /// </summary>
        [HttpPost("filter")]
public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> FilterContents([FromBody] ContentFilterRequest filter)
      {
      try
         {
       var query = _context.Contents.AsQueryable();

       // Tip filtresi (movie/book)
        if (!string.IsNullOrWhiteSpace(filter.Type))
    {
       query = query.Where(c => c.Type == filter.Type.ToLower());
                }

   // Başlık araması
     if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
     {
          query = query.Where(c => c.Title.Contains(filter.SearchTerm));
  }

  // Tür filtresi (JSON içinde arama)
   if (filter.Genres != null && filter.Genres.Any())
  {
        foreach (var genre in filter.Genres)
 {
      query = query.Where(c => c.Genres != null && c.Genres.Contains(genre));
    }
  }

 // Yıl filtresi
         if (filter.MinYear.HasValue)
         {
          query = query.Where(c => c.Year >= filter.MinYear.Value);
     }
       if (filter.MaxYear.HasValue)
           {
        query = query.Where(c => c.Year <= filter.MaxYear.Value);
 }

      // Yönetmen filtresi (filmler için)
     if (!string.IsNullOrWhiteSpace(filter.Director))
           {
 query = query.Where(c => c.Director != null && c.Director.Contains(filter.Director));
    }

          // Yazar filtresi (kitaplar için)
        if (!string.IsNullOrWhiteSpace(filter.Author))
     {
              query = query.Where(c => c.Authors != null && c.Authors.Contains(filter.Author));
            }

      // Toplam sayı
 var totalCount = await query.CountAsync();

 // Sıralama
    query = filter.SortBy?.ToLower() switch
    {
 "title" => filter.SortDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
      "year" => filter.SortDescending ? query.OrderByDescending(c => c.Year) : query.OrderBy(c => c.Year),
 "rating" => query.OrderByDescending(c => c.Ratings.Any() ? c.Ratings.Average(r => r.Score) : 0),
  _ => query.OrderByDescending(c => c.CreatedAt)
     };

         // Pagination
   var pageSize = filter.PageSize > 0 && filter.PageSize <= 50 ? filter.PageSize : 20;
    var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

        var contents = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
        .Include(c => c.Ratings)
     .ToListAsync();

    // DTO'ya map et
    var results = contents.Select(c => new ContentSummaryDto
         {
     Id = c.Id,
   ExternalId = c.ExternalId,
   Type = c.Type,
    Title = c.Title,
      Year = c.Year,
   CoverUrl = c.CoverUrl,
   AverageRating = c.Ratings.Any() ? Math.Round(c.Ratings.Average(r => r.Score), 1) : 0,
     RatingsCount = c.Ratings.Count,
         Director = c.Director,
         Genres = !string.IsNullOrEmpty(c.Genres) 
     ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.Genres) 
          : new List<string>(),
        Authors = !string.IsNullOrEmpty(c.Authors)
? System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.Authors)
       : new List<string>()
   }).ToList();

 // Puan filtresi (DTO'dan sonra)
if (filter.MinRating.HasValue)
      {
        results = results.Where(c => c.AverageRating >= filter.MinRating.Value).ToList();
          totalCount = results.Count;
       }

    var pagedResult = new PagedResult<ContentSummaryDto>(results, totalCount, pageNumber, pageSize);

          return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
 pagedResult,
    $"{totalCount} icerik bulundu."));
      }
     catch (Exception ex)
     {
   _logger.LogError(ex, "FilterContents error");
 return StatusCode(500, ApiResponse<PagedResult<ContentSummaryDto>>.FailResponse(
        "Filtreleme sirasinda bir hata olustu.",
         new List<string> { ex.Message }));
}
      }
    }
}
