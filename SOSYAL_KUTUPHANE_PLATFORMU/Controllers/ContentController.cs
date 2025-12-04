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
    public class ContentController : BaseController
 {
        private readonly ApplicationDbContext _context;
 private readonly ILogger<ContentController> _logger;

        public ContentController(ApplicationDbContext context, ILogger<ContentController> logger)
    {
    _context = context;
            _logger = logger;
  }

        [HttpPost("ensure")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> EnsureContent([FromBody] EnsureContentRequest model)
        {
            try
          {
  _logger.LogInformation($"EnsureContent called - ExternalId: {model?.ExternalId}, Type: {model?.Type}, Title: {model?.Title}");

      // Manuel validation (Required attribute'larý kaldýrdýk)
    if (model == null)
       {
            _logger.LogWarning("EnsureContent: model is null");
    return BadRequest(ApiResponse<object>.FailResponse("Request body bos olamaz."));
     }

      if (string.IsNullOrWhiteSpace(model.ExternalId))
 {
         _logger.LogWarning("EnsureContent: ExternalId is null or empty");
             return BadRequest(ApiResponse<object>.FailResponse("ExternalId zorunludur."));
     }

      if (string.IsNullOrWhiteSpace(model.Type))
         {
              _logger.LogWarning("EnsureContent: Type is null or empty");
          return BadRequest(ApiResponse<object>.FailResponse("Type zorunludur."));
     }

    // Type validasyonu (movie veya book olmalý)
   if (model.Type != "movie" && model.Type != "book")
          {
         _logger.LogWarning($"EnsureContent: Invalid type '{model.Type}'");
   return BadRequest(ApiResponse<object>.FailResponse("Type 'movie' veya 'book' olmalidir."));
         }

      if (string.IsNullOrWhiteSpace(model.Title))
    {
          _logger.LogWarning("EnsureContent: Title is null or empty");
      return BadRequest(ApiResponse<object>.FailResponse("Title zorunludur."));
    }

    // Ayný ExternalId + Type kombinasyonu varsa mevcut kaydý dön
      var existing = await _context.Contents
            .FirstOrDefaultAsync(c => c.ExternalId == model.ExternalId && c.Type == model.Type);

             if (existing != null)
             {
             _logger.LogInformation($"Content already exists - ID: {existing.Id}, ExternalId: {model.ExternalId}");
   return Ok(ApiResponse<object>.SuccessResponse(
    new { contentId = existing.Id },
       "Icerik zaten mevcut."));
   }

    // Description ve Overview alanlarýný birleþtir
string? description = !string.IsNullOrWhiteSpace(model.Description)
      ? model.Description
      : (!string.IsNullOrWhiteSpace(model.Overview) ? model.Overview : null);

          // CoverUrl ve PosterPath alanlarýný birleþtir
     string? coverUrl = !string.IsNullOrWhiteSpace(model.CoverUrl)
  ? model.CoverUrl
            : (!string.IsNullOrWhiteSpace(model.PosterPath) ? model.PosterPath : null);

            // ReleaseDate'den Year çýkar (eðer Year boþsa)
      int? year = model.Year;
 if (!year.HasValue && !string.IsNullOrWhiteSpace(model.ReleaseDate))
   {
   try
      {
               if (model.ReleaseDate.Length >= 4 && int.TryParse(model.ReleaseDate.Substring(0, 4), out int parsedYear))
          {
            year = parsedYear;
  }
           }
         catch (Exception ex)
             {
            _logger.LogWarning(ex, $"Error parsing year from ReleaseDate: {model.ReleaseDate}");
      }
         }

  // Content oluþtur
            var content = new Content
    {
            ExternalId = model.ExternalId.Trim(),
        Type = model.Type.ToLower().Trim(),
         Title = model.Title.Trim(),
          Description = description?.Trim(),
     Year = year,
       CoverUrl = coverUrl?.Trim(),
       // Detaylý alanlar
       Director = model.Director?.Trim(),
          Cast = model.Cast != null && model.Cast.Any()
 ? System.Text.Json.JsonSerializer.Serialize(model.Cast)
        : null,
          Genres = model.Genres != null && model.Genres.Any()
         ? System.Text.Json.JsonSerializer.Serialize(model.Genres)
        : null,
   Authors = model.Authors != null && model.Authors.Any()
           ? System.Text.Json.JsonSerializer.Serialize(model.Authors)
        : null,
         PageCount = model.PageCount,
   CreatedAt = DateTime.UtcNow
 };

         _context.Contents.Add(content);
    await _context.SaveChangesAsync();

    _logger.LogInformation($"Content created successfully - ID: {content.Id}, ExternalId: {model.ExternalId}, Title: {model.Title}");

        return Ok(ApiResponse<object>.SuccessResponse(
    new { contentId = content.Id },
   "Icerik basariyla eklendi."));
            }
            catch (Exception ex)
            {
        _logger.LogError(ex, $"EnsureContent FATAL ERROR - ExternalId: {model?.ExternalId}, Type: {model?.Type}, Title: {model?.Title}");
       _logger.LogError($"Exception Message: {ex.Message}");
  _logger.LogError($"Stack Trace: {ex.StackTrace}");
       _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");

                var errorMessages = new List<string> { ex.Message };
if (ex.InnerException != null)
                {
          errorMessages.Add($"Inner: {ex.InnerException.Message}");
          }

        return StatusCode(500, ApiResponse<object>.FailResponse(
            "Icerik eklenirken bir hata olustu.",
             errorMessages));
 }
        }

  [HttpPost("rate")]
        [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> RateContent([FromBody] RateContentRequest model)
        {
        try
            {
 if (!ModelState.IsValid)
          return BadRequest(ApiResponse<object>.FailResponse("Gecersiz veri", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

    if (model.Score < 1 || model.Score > 10)
       return BadRequest(ApiResponse<object>.FailResponse("Puan 1-10 arasinda olmalidir."));

    var userId = GetCurrentUserId();
         var content = await _context.Contents.FindAsync(model.ContentId);
           if (content == null)
            {
           _logger.LogWarning($"Content not found with ID: {model.ContentId}");
   return NotFound(ApiResponse<object>.FailResponse("Icerik bulunamadi."));
           }

     var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == model.ContentId);
        bool isNewRating = rating == null;

                if (rating == null)
   {
            rating = new Rating { UserId = userId, ContentId = model.ContentId, Score = model.Score, CreatedAt = DateTime.UtcNow };
             _context.Ratings.Add(rating);
      }
        else
    {
   rating.Score = model.Score;
      rating.UpdatedAt = DateTime.UtcNow;
      _context.Ratings.Update(rating);
   }

      await _context.SaveChangesAsync();

  if (isNewRating)
        {
      var activity = new Activity { UserId = userId, ContentId = model.ContentId, ActivityType = "rating", Score = model.Score, CreatedAt = DateTime.UtcNow };
        _context.Activities.Add(activity);
                await _context.SaveChangesAsync();
            }

return Ok(ApiResponse<object>.SuccessResponse(new { ratingId = rating.Id, score = rating.Score, isUpdate = !isNewRating }, isNewRating ? "Puan basariyla kaydedildi." : "Puan guncellendi."));
    }
       catch (Exception ex)
            {
              _logger.LogError(ex, $"RateContent error: {ex.Message}");
           return StatusCode(500, ApiResponse<object>.FailResponse("Puanlama sirasinda bir hata olustu.", new List<string> { ex.Message }));
            }
  }

        [HttpPost("review")]
        [Authorize]
 public async Task<ActionResult<ApiResponse<object>>> AddReview([FromBody] ReviewContentRequest model)
 {
            try
            {
          if (!ModelState.IsValid)
        return BadRequest(ApiResponse<object>.FailResponse("Gecersiz veri", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

     var userId = GetCurrentUserId();
       var content = await _context.Contents.FindAsync(model.ContentId);
    if (content == null)
                {
  _logger.LogWarning($"Content not found with ID: {model.ContentId}");
         return NotFound(ApiResponse<object>.FailResponse("Icerik bulunamadi."));
  }

                var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == model.ContentId);
      if (existingReview != null)
         return BadRequest(ApiResponse<object>.FailResponse("Bu icerik hakkinda zaten yorum yaptiniz. Yorumunuzu duzenleyebilirsiniz."));

           var review = new Review { UserId = userId, ContentId = model.ContentId, Text = model.Text, CreatedAt = DateTime.UtcNow };
    _context.Reviews.Add(review);
      await _context.SaveChangesAsync();

       var activity = new Activity { UserId = userId, ContentId = model.ContentId, ActivityType = "review", Text = model.Text, CreatedAt = DateTime.UtcNow };
          _context.Activities.Add(activity);
    await _context.SaveChangesAsync();

          return Ok(ApiResponse<object>.SuccessResponse(new { reviewId = review.Id }, "Yorum basariyla kaydedildi."));
            }
        catch (Exception ex)
            {
      _logger.LogError(ex, $"AddReview error: {ex.Message}");
    return StatusCode(500, ApiResponse<object>.FailResponse("Yorum eklenirken bir hata olustu.", new List<string> { ex.Message }));
      }
 }

        [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContentDetailsDto>>> GetContentDetails(int id)
 {
         try
            {
     var content = await _context.Contents.Include(c => c.Ratings).Include(c => c.Reviews).ThenInclude(r => r.User).FirstOrDefaultAsync(c => c.Id == id);
     if (content == null)
         {
         _logger.LogWarning($"Content not found with ID: {id}");
       return NotFound(ApiResponse<ContentDetailsDto>.FailResponse("Icerik bulunamadi."));
   }

              double avgRating = 0;
int ratingsCount = content.Ratings.Count;
  int reviewsCount = content.Reviews.Count;
      if (ratingsCount > 0) avgRating = content.Ratings.Average(r => r.Score);

           int listAddCount = await _context.UserListItems.Where(uli => uli.ContentId == id).CountAsync();
         int? currentUserRating = null;
                bool hasUserReviewed = false;
      UserLibraryStatusDto? userLibraryStatus = null;

      if (User.Identity?.IsAuthenticated == true)
       {
    var userId = GetCurrentUserId();
         var userRating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == id);
     currentUserRating = userRating?.Score;
         hasUserReviewed = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ContentId == id);

            var userLists = await _context.UserLists.Where(ul => ul.UserId == userId).Include(ul => ul.Items).ToListAsync();
var userListsWithContent = userLists.Where(ul => ul.Items.Any(item => item.ContentId == id)).ToList();
     userLibraryStatus = new UserLibraryStatusDto
 {
         IsInWatchedList = userListsWithContent.Any(ul => ul.Name == "Izlediklerim" && ul.IsDefault),
  IsInToWatchList = userListsWithContent.Any(ul => ul.Name == "Izlenecekler" && ul.IsDefault),
          IsInReadList = userListsWithContent.Any(ul => ul.Name == "Okuduklarim" && ul.IsDefault),
          IsInToReadList = userListsWithContent.Any(ul => ul.Name == "Okunacaklar" && ul.IsDefault),
        CustomLists = userListsWithContent.Where(ul => !ul.IsDefault).Select(ul => ul.Name).ToList()
       };
           }

                // JSON string'leri deserialize et
     List<string>? cast = null;
    List<string>? genres = null;
              List<string>? authors = null;

   try
        {
 if (!string.IsNullOrEmpty(content.Cast))
      cast = System.Text.Json.JsonSerializer.Deserialize<List<string>>(content.Cast);
              if (!string.IsNullOrEmpty(content.Genres))
            genres = System.Text.Json.JsonSerializer.Deserialize<List<string>>(content.Genres);
   if (!string.IsNullOrEmpty(content.Authors))
     authors = System.Text.Json.JsonSerializer.Deserialize<List<string>>(content.Authors);
      }
      catch (Exception ex)
                {
       _logger.LogWarning(ex, "Error deserializing JSON fields for content {ContentId}", id);
       }

             var dto = new ContentDetailsDto
    {
               Id = content.Id,
   ExternalId = content.ExternalId,
 Type = content.Type,
 Title = content.Title,
     Description = content.Description,
          Year = content.Year,
            CoverUrl = content.CoverUrl,
    // Detaylý alanlar
          Director = content.Director,
        Cast = cast,
         Genres = genres,
           Authors = authors,
            PageCount = content.PageCount,
       // Ýstatistikler
       AverageRating = Math.Round(avgRating, 2),
      RatingsCount = ratingsCount,
      ReviewsCount = reviewsCount,
          ListAddCount = listAddCount,
 CurrentUserRating = currentUserRating,
           HasUserReviewed = hasUserReviewed,
          UserLibraryStatus = userLibraryStatus,
         Reviews = content.Reviews.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewDto
   {
               Id = r.Id,
   UserId = r.UserId,
        UserName = r.User.UserName,
         UserAvatarUrl = r.User.AvatarUrl,
       Text = r.Text,
  CreatedAt = r.CreatedAt,
             UpdatedAt = r.UpdatedAt
           }).ToList()
      };

             return Ok(ApiResponse<ContentDetailsDto>.SuccessResponse(dto, "Icerik detaylari basariyla getirildi."));
   }
        catch (Exception ex)
            {
           _logger.LogError(ex, $"GetContentDetails error for ID {id}: {ex.Message}");
        return StatusCode(500, ApiResponse<ContentDetailsDto>.FailResponse("Icerik detaylari getirilirken bir hata olustu.", new List<string> { ex.Message }));
    }
        }

    [HttpGet("{contentId:int}/reviews")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReviewDto>>>> GetContentReviewsPaginated(int contentId, [FromQuery] int page = 1, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
  try
   {
        _logger.LogInformation($"GetContentReviews called for ContentId: {contentId}, Page: {page}, PageNumber: {pageNumber}");

         int actualPage = page > 1 ? page : pageNumber;
    if (actualPage < 1) actualPage = 1;
     if (pageSize <= 0 || pageSize > 100) pageSize = 10;

      var contentExists = await _context.Contents.AnyAsync(c => c.Id == contentId);
if (!contentExists)
            {
    _logger.LogInformation($"Content with ID {contentId} not found - returning empty reviews");
            var emptyResult = new PagedResult<ReviewDto>(new List<ReviewDto>(), 0, actualPage, pageSize);
        return Ok(ApiResponse<PagedResult<ReviewDto>>.SuccessResponse(
  emptyResult,
          "Bu icerik henuz veritabaninda yok - yorum bulunmuyor."));
         }

      var totalCount = await _context.Reviews.Where(r => r.ContentId == contentId).CountAsync();
            var reviews = await _context.Reviews
         .Where(r => r.ContentId == contentId)
     .Include(r => r.User)
        .OrderByDescending(r => r.CreatedAt)
        .Skip((actualPage - 1) * pageSize)
         .Take(pageSize)
     .Select(r => new ReviewDto
        {
         Id = r.Id,
        UserId = r.UserId,
          UserName = r.User.UserName,
      UserAvatarUrl = r.User.AvatarUrl,
       Text = r.Text,
            CreatedAt = r.CreatedAt,
         UpdatedAt = r.UpdatedAt
              })
         .ToListAsync();

     _logger.LogInformation($"Returning {reviews.Count} reviews for ContentId: {contentId}");

   var pagedResult = new PagedResult<ReviewDto>(reviews, totalCount, actualPage, pageSize);
      return Ok(ApiResponse<PagedResult<ReviewDto>>.SuccessResponse(
   pagedResult,
           $"Icerik yorumlari basariyla getirildi. Toplam {totalCount} yorum."));
  }
     catch (Exception ex)
   {
     _logger.LogError(ex, $"GetContentReviews error for ContentId {contentId}: {ex.Message}");
                return StatusCode(500, ApiResponse<PagedResult<ReviewDto>>.FailResponse(
            "Yorumlar getirilirken bir hata olustu.",
    new List<string> { ex.Message }));
     }
  }

        [HttpGet("{contentId:int}/yorumlar")]
      public async Task<ActionResult<ApiResponse<PagedResult<ReviewDto>>>> GetContentReviewsTurkish(int contentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
          try
            {
     if (pageSize <= 0 || pageSize > 100) pageSize = 10;
         if (pageNumber <= 0) pageNumber = 1;

       var contentExists = await _context.Contents.AnyAsync(c => c.Id == contentId);
                if (!contentExists)
 {
        _logger.LogWarning($"Content not found with ID: {contentId}");
                    return NotFound(ApiResponse<PagedResult<ReviewDto>>.FailResponse("Icerik bulunamadi."));
      }

      var totalCount = await _context.Reviews.Where(r => r.ContentId == contentId).CountAsync();
       var reviews = await _context.Reviews.Where(r => r.ContentId == contentId).Include(r => r.User).OrderByDescending(r => r.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize)
             .Select(r => new ReviewDto { Id = r.Id, UserId = r.UserId, UserName = r.User.UserName, UserAvatarUrl = r.User.AvatarUrl, Text = r.Text, CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt }).ToListAsync();

         var pagedResult = new PagedResult<ReviewDto>(reviews, totalCount, pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<ReviewDto>>.SuccessResponse(pagedResult, $"Icerik yorumlari basariyla getirildi. Toplam {totalCount} yorum."));
       }
       catch (Exception ex)
            {
          _logger.LogError(ex, $"GetContentReviews error for ContentId {contentId}: {ex.Message}");
 return StatusCode(500, ApiResponse<PagedResult<ReviewDto>>.FailResponse("Yorumlar getirilirken bir hata olustu.", new List<string> { ex.Message }));
      }
        }

        [HttpPut("review/{id:int}")]
        [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateReview(int id, [FromBody] ReviewContentRequest model)
        {
    try
  {
           if (!ModelState.IsValid)
  return BadRequest(ApiResponse.FailResponse("Gecersiz veri", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

       var userId = GetCurrentUserId();
          var review = await _context.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
     if (review == null)
                {
_logger.LogWarning($"Review not found with ID: {id}");
           return NotFound(ApiResponse.FailResponse("Yorum bulunamadi."));
     }
        if (review.UserId != userId)
      {
     _logger.LogWarning($"User {userId} attempted to update review {id} owned by {review.UserId}");
      return Forbid();
  }

    review.Text = model.Text;
   review.UpdatedAt = DateTime.UtcNow;
                _context.Reviews.Update(review);
      await _context.SaveChangesAsync();

        var activity = await _context.Activities.FirstOrDefaultAsync(a => a.UserId == userId && a.ContentId == review.ContentId && a.ActivityType == "review");
    if (activity != null)
      {
          activity.Text = model.Text;
          _context.Activities.Update(activity);
       await _context.SaveChangesAsync();
                }

          return Ok(ApiResponse.SuccessResponse("Yorum basariyla guncellendi."));
 }
       catch (Exception ex)
 {
        _logger.LogError(ex, $"UpdateReview error for ID {id}: {ex.Message}");
         return StatusCode(500, ApiResponse.FailResponse("Yorum guncellenirken bir hata olustu.", new List<string> { ex.Message }));
   }
        }

    [HttpDelete("review/{id:int}")]
        [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteReview(int id)
        {
        try
          {
            var userId = GetCurrentUserId();
     var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
      if (review == null)
    {
         _logger.LogWarning($"Review not found with ID: {id}");
       return NotFound(ApiResponse.FailResponse("Yorum bulunamadi."));
          }
    if (review.UserId != userId)
             {
       _logger.LogWarning($"User {userId} attempted to delete review {id} owned by {review.UserId}");
   return Forbid();
     }

           _context.Reviews.Remove(review);
        var activity = await _context.Activities.FirstOrDefaultAsync(a => a.UserId == userId && a.ContentId == review.ContentId && a.ActivityType == "review");
      if (activity != null)
     _context.Activities.Remove(activity);

     await _context.SaveChangesAsync();
             return Ok(ApiResponse.SuccessResponse("Yorum basariyla silindi."));
    }
     catch (Exception ex)
{
          _logger.LogError(ex, $"DeleteReview error for ID {id}: {ex.Message}");
                return StatusCode(500, ApiResponse.FailResponse("Yorum silinirken bir hata olustu.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{contentId:int}/my-rating")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetMyRating(int contentId)
        {
   try
            {
 var userId = GetCurrentUserId();

      var contentExists = await _context.Contents.AnyAsync(c => c.Id == contentId);
   if (!contentExists)
     {
   _logger.LogInformation($"Content with ID {contentId} not found - no rating available");
           return Ok(ApiResponse<object>.SuccessResponse(null, "Bu icerik henuz veritabaninda yok."));
              }

var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);
     if (rating == null)
                {
          _logger.LogInformation($"No rating found for User {userId} on Content {contentId}");
             return Ok(ApiResponse<object>.SuccessResponse(null, "Bu iceriye henuz puan vermediniz."));
   }

    return Ok(ApiResponse<object>.SuccessResponse(new { score = rating.Score, createdAt = rating.CreatedAt, updatedAt = rating.UpdatedAt }, "Puaniniz basariyla getirildi."));
            }
            catch (Exception ex)
       {
         _logger.LogError(ex, $"GetMyRating error for ContentId {contentId}: {ex.Message}");
  return StatusCode(500, ApiResponse<object>.FailResponse("Puan getirilirken bir hata olustu.", new List<string> { ex.Message }));
 }
        }

        [HttpGet("{contentId:int}/my-review")]
        [Authorize]
   public async Task<ActionResult<ApiResponse<object>>> GetMyReview(int contentId)
  {
            try
            {
        var userId = GetCurrentUserId();
           var review = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);
    if (review == null)
{
      _logger.LogInformation($"No review found for User {userId} on Content {contentId}");
          return NotFound(ApiResponse<object>.FailResponse("Bu icerik hakkinda henuz yorum yapmadiniz."));
}

 return Ok(ApiResponse<object>.SuccessResponse(new { id = review.Id, text = review.Text, createdAt = review.CreatedAt, updatedAt = review.UpdatedAt }, "Yorumunuz basariyla getirildi."));
      }
            catch (Exception ex)
        {
       _logger.LogError(ex, $"GetMyReview error for ContentId {contentId}: {ex.Message}");
    return StatusCode(500, ApiResponse<object>.FailResponse("Yorum getirilirken bir hata olustu.", new List<string> { ex.Message }));
   }
        }

        /// <summary>
        /// Ýçeriðin kullanýcýlar tarafýndan verilen ortalama puanýný getir
        /// GET: api/content/{contentId}/average-rating
        /// </summary>
  [HttpGet("{contentId:int}/average-rating")]
     public async Task<ActionResult<ApiResponse<object>>> GetAverageRating(int contentId)
  {
         try
         {
                var ratings = await _context.Ratings
  .Where(r => r.ContentId == contentId)
  .ToListAsync();

       if (!ratings.Any())
       {
   return Ok(ApiResponse<object>.SuccessResponse(
      new { averageRating = 0.0, ratingsCount = 0 },
          "Bu icerik henuz puanlanmamis."));
 }

            var average = ratings.Average(r => r.Score);

  return Ok(ApiResponse<object>.SuccessResponse(
         new
          {
       averageRating = Math.Round(average, 1),
         ratingsCount = ratings.Count
  },
       "Ortalama puan basariyla getirildi."));
          }
            catch (Exception ex)
      {
      _logger.LogError(ex, $"GetAverageRating error for ContentId {contentId}: {ex.Message}");
      return StatusCode(500, ApiResponse<object>.FailResponse(
     "Ortalama puan getirilirken bir hata olustu.",
   new List<string> { ex.Message }));
    }
        }
    }
}
