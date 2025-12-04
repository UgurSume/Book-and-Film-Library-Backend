using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscoverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscoverController(ApplicationDbContext context)
        {
        _context = context;
        }

        /// <summary>
   /// En yüksek puanlý içerikler
        /// GET: api/discover/top-rated?type=movie&pageNumber=1&pageSize=20&minRatings=5
        /// </summary>
        [HttpGet("top-rated")]
   public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> GetTopRated(
      [FromQuery] string? type = null,
     [FromQuery] int pageNumber = 1,
[FromQuery] int pageSize = 20,
            [FromQuery] int minRatings = 5)
        {
 if (pageSize <= 0 || pageSize > 100) pageSize = 20;
  if (pageNumber <= 0) pageNumber = 1;

         var query = _context.Contents
            .Include(c => c.Ratings)
      .Include(c => c.Reviews)
    .Where(c => c.Ratings.Count >= minRatings);

       if (!string.IsNullOrEmpty(type))
 {
       query = query.Where(c => c.Type == type);
  }

            var totalCount = await query.CountAsync();

            var contents = await query
          .Select(c => new
    {
      Content = c,
          AvgRating = c.Ratings.Average(r => (double)r.Score),
     RatingsCount = c.Ratings.Count,
         ReviewsCount = c.Reviews.Count,
    ListAddCount = _context.UserListItems.Count(i => i.ContentId == c.Id)
         })
              .OrderByDescending(x => x.AvgRating)
       .ThenByDescending(x => x.RatingsCount)
                .Skip((pageNumber - 1) * pageSize)
  .Take(pageSize)
       .ToListAsync();

var result = contents.Select(x => new ContentSummaryDto
  {
    Id = x.Content.Id,
         ExternalId = x.Content.ExternalId,
    Type = x.Content.Type,
     Title = x.Content.Title,
       Description = x.Content.Description,
                Year = x.Content.Year,
    CoverUrl = x.Content.CoverUrl,
   AverageRating = Math.Round(x.AvgRating, 2),
                RatingsCount = x.RatingsCount,
   ReviewsCount = x.ReviewsCount,
    ListAddCount = x.ListAddCount,
                CreatedAt = x.Content.CreatedAt
            }).ToList();

     var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);
     return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
    pagedResult, "En yüksek puanlý içerikler baþarýyla getirildi."));
        }

/// <summary>
        /// En popüler içerikler (En çok etkileþim alan)
/// GET: api/discover/popular?type=book&pageNumber=1&pageSize=20
        /// </summary>
        [HttpGet("popular")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> GetPopular(
    [FromQuery] string? type = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
     {
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
         if (pageNumber <= 0) pageNumber = 1;

     var query = _context.Contents
   .Include(c => c.Ratings)
                .Include(c => c.Reviews)
.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
           query = query.Where(c => c.Type == type);
       }

  var allContents = await query
         .Select(c => new
                {
           Content = c,
          RatingsCount = c.Ratings.Count,
     ReviewsCount = c.Reviews.Count,
   ListAddCount = _context.UserListItems.Count(i => i.ContentId == c.Id),
  AvgRating = c.Ratings.Any() ? c.Ratings.Average(r => (double)r.Score) : 0,
    PopularityScore = (c.Ratings.Count * 1.0) + 
               (c.Reviews.Count * 2.0) + 
    (_context.UserListItems.Count(i => i.ContentId == c.Id) * 1.5)
        })
     .Where(x => x.PopularityScore > 0)
      .OrderByDescending(x => x.PopularityScore)
       .ToListAsync();

      var totalCount = allContents.Count;
        var contents = allContents
   .Skip((pageNumber - 1) * pageSize)
       .Take(pageSize)
     .ToList();

    var result = contents.Select(x => new ContentSummaryDto
        {
          Id = x.Content.Id,
     ExternalId = x.Content.ExternalId,
            Type = x.Content.Type,
        Title = x.Content.Title,
      Description = x.Content.Description,
                Year = x.Content.Year,
                CoverUrl = x.Content.CoverUrl,
  AverageRating = Math.Round(x.AvgRating, 2),
             RatingsCount = x.RatingsCount,
  ReviewsCount = x.ReviewsCount,
       ListAddCount = x.ListAddCount,
    CreatedAt = x.Content.CreatedAt
            }).ToList();

     var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);
   return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
pagedResult, "Popüler içerikler baþarýyla getirildi."));
      }

  /// <summary>
        /// Trend olan içerikler (Son X günün en popülerleri)
        /// GET: api/discover/trending?type=movie&days=7&pageNumber=1&pageSize=20
        /// </summary>
        [HttpGet("trending")]
    public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> GetTrending(
      [FromQuery] string? type = null,
 [FromQuery] int days = 7,
   [FromQuery] int pageNumber = 1,
   [FromQuery] int pageSize = 20)
{
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;
            if (days <= 0 || days > 30) days = 7;

   var sinceDate = DateTime.UtcNow.AddDays(-days);

            var query = _context.Contents
     .Include(c => c.Ratings)
      .Include(c => c.Reviews)
     .AsQueryable();

   if (!string.IsNullOrEmpty(type))
  {
   query = query.Where(c => c.Type == type);
            }

   var allContents = await query
.Select(c => new
         {
        Content = c,
          RecentRatingsCount = c.Ratings.Count(r => r.CreatedAt >= sinceDate),
 RecentReviewsCount = c.Reviews.Count(r => r.CreatedAt >= sinceDate),
         TotalRatingsCount = c.Ratings.Count,
   TotalReviewsCount = c.Reviews.Count,
   AvgRating = c.Ratings.Any() ? c.Ratings.Average(r => (double)r.Score) : 0,
    TrendScore = (c.Ratings.Count(r => r.CreatedAt >= sinceDate) * 2.0) + 
      (c.Reviews.Count(r => r.CreatedAt >= sinceDate) * 3.0) + 
    (_context.Activities.Count(a => a.ContentId == c.Id && 
        a.ActivityType == "add_to_list" && 
     a.CreatedAt >= sinceDate) * 1.5)
         })
           .Where(x => x.TrendScore > 0)
            .OrderByDescending(x => x.TrendScore)
          .ToListAsync();

  var totalCount = allContents.Count;
            var contents = allContents
        .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
         .ToList();

  var result = contents.Select(x => new ContentSummaryDto
          {
         Id = x.Content.Id,
                ExternalId = x.Content.ExternalId,
        Type = x.Content.Type,
  Title = x.Content.Title,
        Description = x.Content.Description,
      Year = x.Content.Year,
      CoverUrl = x.Content.CoverUrl,
          AverageRating = Math.Round(x.AvgRating, 2),
 RatingsCount = x.TotalRatingsCount,
    ReviewsCount = x.TotalReviewsCount,
    ListAddCount = _context.UserListItems.Count(i => i.ContentId == x.Content.Id),
     CreatedAt = x.Content.CreatedAt
   }).ToList();

            var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);
          return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
  pagedResult, $"Son {days} günün trend içerikleri baþarýyla getirildi."));
        }

    /// <summary>
     /// Geliþmiþ filtreleme (Proje metni gereksinimi)
    /// POST: api/discover/filter
   /// </summary>
        [HttpPost("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> FilterContents(
     [FromBody] ContentFilterRequest filter)
        {
      if (!ModelState.IsValid)
 {
       return BadRequest(ApiResponse<PagedResult<ContentSummaryDto>>.FailResponse(
"Geçersiz filtre parametreleri", 
    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
      }

         var query = _context.Contents
 .Include(c => c.Ratings)
    .Include(c => c.Reviews)
    .AsQueryable();

    // Tip filtresi
      if (!string.IsNullOrEmpty(filter.Type))
     {
       query = query.Where(c => c.Type == filter.Type);
}

     // Yýl filtresi
        if (filter.MinYear.HasValue)
   {
        query = query.Where(c => c.Year >= filter.MinYear.Value);
         }
     if (filter.MaxYear.HasValue)
            {
  query = query.Where(c => c.Year <= filter.MaxYear.Value);
       }

var contents = await query
    .Select(c => new
        {
    Content = c,
  AvgRating = c.Ratings.Any() ? c.Ratings.Average(r => (double)r.Score) : 0,
        RatingsCount = c.Ratings.Count,
       ReviewsCount = c.Reviews.Count,
       ListAddCount = _context.UserListItems.Count(i => i.ContentId == c.Id)
    })
  .ToListAsync();

    // Puan filtresi (in-memory)
        if (filter.MinRating.HasValue)
            {
        contents = contents.Where(x => x.AvgRating >= filter.MinRating.Value).ToList();
     }

            if (filter.MaxRating.HasValue)
    {
      contents = contents.Where(x => x.AvgRating <= filter.MaxRating.Value).ToList();
        }

          // Sýralama
 contents = filter.SortBy?.ToLower() switch
          {
       "title" => filter.SortDescending ? contents.OrderByDescending(x => x.Content.Title).ToList() : contents.OrderBy(x => x.Content.Title).ToList(),
       "year" => filter.SortDescending ? contents.OrderByDescending(x => x.Content.Year).ToList() : contents.OrderBy(x => x.Content.Year).ToList(),
            "rating" => filter.SortDescending ? contents.OrderByDescending(x => x.AvgRating).ToList() : contents.OrderBy(x => x.AvgRating).ToList(),
   "created" => filter.SortDescending ? contents.OrderByDescending(x => x.Content.CreatedAt).ToList() : contents.OrderBy(x => x.Content.CreatedAt).ToList(),
       _ => contents.OrderByDescending(x => x.AvgRating).ToList()
            };

    var totalCount = contents.Count;

    // Sayfalama
   var pageSize = filter.PageSize > 0 && filter.PageSize <= 50 ? filter.PageSize : 20;
        var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

        var pagedContents = contents
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
  .ToList();

  var result = pagedContents.Select(x => new ContentSummaryDto
  {
Id = x.Content.Id,
   ExternalId = x.Content.ExternalId,
     Type = x.Content.Type,
  Title = x.Content.Title,
  Description = x.Content.Description,
        Year = x.Content.Year,
   CoverUrl = x.Content.CoverUrl,
   AverageRating = Math.Round(x.AvgRating, 2),
    RatingsCount = x.RatingsCount,
     ReviewsCount = x.ReviewsCount,
     ListAddCount = x.ListAddCount,
  CreatedAt = x.Content.CreatedAt
}).ToList();

      var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);

    return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
    pagedResult, "Filtrelenmiþ içerikler baþarýyla getirildi."));
        }

/// <summary>
      /// Yeni eklenen içerikler
        /// GET: api/discover/recent?type=movie&pageNumber=1&pageSize=20
        /// </summary>
  [HttpGet("recent")]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> GetRecent(
            [FromQuery] string? type = null,
            [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
        {
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
          if (pageNumber <= 0) pageNumber = 1;

var query = _context.Contents
      .Include(c => c.Ratings)
  .Include(c => c.Reviews)
      .AsQueryable();

            if (!string.IsNullOrEmpty(type))
       {
          query = query.Where(c => c.Type == type);
            }

  var totalCount = await query.CountAsync();

     var contents = await query
     .OrderByDescending(c => c.CreatedAt)
   .Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
    .Select(c => new
     {
       Content = c,
              AvgRating = c.Ratings.Any() ? c.Ratings.Average(r => (double)r.Score) : 0,
    RatingsCount = c.Ratings.Count,
       ReviewsCount = c.Reviews.Count,
   ListAddCount = _context.UserListItems.Count(i => i.ContentId == c.Id)
      })
             .ToListAsync();

var result = contents.Select(x => new ContentSummaryDto
            {
     Id = x.Content.Id,
ExternalId = x.Content.ExternalId,
          Type = x.Content.Type,
     Title = x.Content.Title,
             Description = x.Content.Description,
   Year = x.Content.Year,
         CoverUrl = x.Content.CoverUrl,
          AverageRating = Math.Round(x.AvgRating, 2),
    RatingsCount = x.RatingsCount,
     ReviewsCount = x.ReviewsCount,
    ListAddCount = x.ListAddCount,
   CreatedAt = x.Content.CreatedAt
      }).ToList();

      var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
       pagedResult, "Yeni eklenen içerikler baþarýyla getirildi."));
     }

     /// <summary>
        /// Benzer zevklere sahip kullanýcýlarýn beðendiði içerikler (Giriþ gerekli)
        /// GET: api/discover/recommended?pageNumber=1&pageSize=20
        /// </summary>
        [HttpGet("recommended")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<ContentSummaryDto>>>> GetRecommended(
            [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 20)
        {
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
  if (pageNumber <= 0) pageNumber = 1;

            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

         // Kullanýcýnýn yüksek puan verdiði içerikler (7+)
  var userHighRatedContentIds = await _context.Ratings
          .Where(r => r.UserId == currentUserId && r.Score >= 7)
          .Select(r => r.ContentId)
      .ToListAsync();

       if (!userHighRatedContentIds.Any())
        {
       // Eðer kullanýcýnýn puaný yoksa, popüler içerikleri döndür
                return await GetPopular(null, pageNumber, pageSize);
            }

            // Bu içeriklere de yüksek puan veren diðer kullanýcýlar
      var similarUserIds = await _context.Ratings
   .Where(r => userHighRatedContentIds.Contains(r.ContentId) && 
         r.UserId != currentUserId && 
  r.Score >= 7)
      .Select(r => r.UserId)
  .Distinct()
       .ToListAsync();

      if (!similarUserIds.Any())
{
      return await GetPopular(null, pageNumber, pageSize);
         }

         // Kullanýcýnýn henüz puanlamadýðý içeriklerden, benzer kullanýcýlarýn beðendiði içerikler
        var userRatedContentIds = await _context.Ratings
.Where(r => r.UserId == currentUserId)
     .Select(r => r.ContentId)
       .ToListAsync();

            var allRecommendedContents = await _context.Ratings
                .Where(r => similarUserIds.Contains(r.UserId) && 
                    !userRatedContentIds.Contains(r.ContentId) &&
      r.Score >= 7)
         .GroupBy(r => r.ContentId)
             .Select(g => new
              {
           ContentId = g.Key,
        RecommendationScore = g.Count()
         })
  .OrderByDescending(x => x.RecommendationScore)
       .ToListAsync();

       var totalCount = allRecommendedContents.Count;
   var contentIds = allRecommendedContents
             .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
 .Select(x => x.ContentId)
         .ToList();

       var contents = await _context.Contents
      .Where(c => contentIds.Contains(c.Id))
 .Include(c => c.Ratings)
     .Include(c => c.Reviews)
 .Select(c => new
{
           Content = c,
     AvgRating = c.Ratings.Any() ? c.Ratings.Average(r => (double)r.Score) : 0,
   RatingsCount = c.Ratings.Count,
     ReviewsCount = c.Reviews.Count,
           ListAddCount = _context.UserListItems.Count(i => i.ContentId == c.Id)
              })
    .ToListAsync();

            var result = contents.Select(x => new ContentSummaryDto
    {
          Id = x.Content.Id,
         ExternalId = x.Content.ExternalId,
                Type = x.Content.Type,
    Title = x.Content.Title,
            Description = x.Content.Description,
    Year = x.Content.Year,
     CoverUrl = x.Content.CoverUrl,
          AverageRating = Math.Round(x.AvgRating, 2),
           RatingsCount = x.RatingsCount,
         ReviewsCount = x.ReviewsCount,
    ListAddCount = x.ListAddCount,
          CreatedAt = x.Content.CreatedAt
            }).ToList();

            var pagedResult = new PagedResult<ContentSummaryDto>(result, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ContentSummaryDto>>.SuccessResponse(
      pagedResult, "Önerilen içerikler baþarýyla getirildi."));
        }
    }
}
