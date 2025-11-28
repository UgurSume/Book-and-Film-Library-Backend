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

        public ContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Iceriği veritabaninda garantiye al
        /// </summary>
        // POST: api/content/ensure
        [HttpPost("ensure")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<Content>>> EnsureContent([FromBody] EnsureContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Content>.FailResponse(
                    "Gecersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var existing = await _context.Contents
                .FirstOrDefaultAsync(c => c.ExternalId == model.ExternalId && c.Type == model.Type);

            if (existing != null)
            {
                return Ok(ApiResponse<Content>.SuccessResponse(existing, "Icerik zaten mevcut."));
            }

            var content = new Content
            {
                ExternalId = model.ExternalId,
                Type = model.Type,
                Title = model.Title,
                Description = model.Description,
                Year = model.Year,
                CoverUrl = model.CoverUrl
            };

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Content>.SuccessResponse(content, "Icerik basariyla eklendi."));
        }

        /// <summary>
        /// Puan verme (varsa guncelle, yoksa ekle)
        /// </summary>
        // POST: api/content/rate
        [HttpPost("rate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> RateContent([FromBody] RateContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Gecersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var content = await _context.Contents.FindAsync(model.ContentId);
            if (content == null)
                return NotFound(ApiResponse<object>.FailResponse("Icerik bulunamadi."));

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == model.ContentId);

            bool isNewRating = rating == null;

            if (rating == null)
            {
                rating = new Rating
                {
                    UserId = userId,
                    ContentId = model.ContentId,
                    Score = model.Score,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Ratings.Add(rating);
            }
            else
            {
                rating.Score = model.Score;
                rating.UpdatedAt = DateTime.UtcNow;
                _context.Ratings.Update(rating);
            }

            await _context.SaveChangesAsync();

            // Aktivite kaydı oluştur (sadece yeni puan için)
            if (isNewRating)
            {
                var activity = new Activity
                {
                    UserId = userId,
                    ContentId = model.ContentId,
                    ActivityType = "rating",
                    Score = model.Score,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                ratingId = rating.Id,
                score = rating.Score,
                isUpdate = !isNewRating
            }, isNewRating ? "Puan basariyla kaydedildi." : "Puan guncellendi."));
        }


        /// <summary>
        /// Yorum ekleme
        /// </summary>
        // POST: api/content/review
        [HttpPost("review")]
    [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> AddReview([FromBody] ReviewContentRequest model)
  {
    if (!ModelState.IsValid)
     {
return BadRequest(ApiResponse<object>.FailResponse(
"Gecersiz veri",
   ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
   }

var userId = GetCurrentUserId();

var content = await _context.Contents.FindAsync(model.ContentId);
      if (content == null)
     return NotFound(ApiResponse<object>.FailResponse("Icerik bulunamadi."));

    var existingReview = await _context.Reviews
.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == model.ContentId);

if (existingReview != null)
  {
    return BadRequest(ApiResponse<object>.FailResponse(
 "Bu icerik hakkinda zaten yorum yaptiniz. Yorumunuzu duzenleyebilirsiniz."));
 }

var review = new Review
  {
  UserId = userId,
          ContentId = model.ContentId,
       Text = model.Text,
    CreatedAt = DateTime.UtcNow
       };

  _context.Reviews.Add(review);
     await _context.SaveChangesAsync();

        var activity = new Activity
   {
       UserId = userId,
   ContentId = model.ContentId,
      ActivityType = "review",
  Text = model.Text,
      CreatedAt = DateTime.UtcNow
       };

      _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

 return Ok(ApiResponse<object>.SuccessResponse(new
  {
  reviewId = review.Id
     }, "Yorum basariyla kaydedildi."));
      }

        /// <summary>
        /// Icerik detay + ortalama puan + yorumlar + kullanici durumu
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ContentDetailsDto>>> GetContentDetails(int id)
 {
    var content = await _context.Contents
         .Include(c => c.Ratings)
 .Include(c => c.Reviews)
         .ThenInclude(r => r.User)
  .FirstOrDefaultAsync(c => c.Id == id);

 if (content == null)
   return NotFound(ApiResponse<ContentDetailsDto>.FailResponse("Icerik bulunamadi."));

         double avgRating = 0;
      int ratingsCount = content.Ratings.Count;
int reviewsCount = content.Reviews.Count;

   if (ratingsCount > 0)
     {
avgRating = content.Ratings.Average(r => r.Score);
     }

   int listAddCount = await _context.UserListItems
    .Where(uli => uli.ContentId == id)
             .CountAsync();

      int? currentUserRating = null;
         bool hasUserReviewed = false;
        UserLibraryStatusDto? userLibraryStatus = null;

 if (User.Identity?.IsAuthenticated == true)
    {
  var userId = GetCurrentUserId();

    var userRating = await _context.Ratings
           .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == id);
   currentUserRating = userRating?.Score;

          hasUserReviewed = await _context.Reviews
         .AnyAsync(r => r.UserId == userId && r.ContentId == id);

     var userLists = await _context.UserLists
  .Where(ul => ul.UserId == userId)
   .Include(ul => ul.Items)
     .ToListAsync();

     var userListsWithContent = userLists
 .Where(ul => ul.Items.Any(item => item.ContentId == id))
       .ToList();

    userLibraryStatus = new UserLibraryStatusDto
       {
   IsInWatchedList = userListsWithContent.Any(ul => ul.Name == "Izlediklerim" && ul.IsDefault),
    IsInToWatchList = userListsWithContent.Any(ul => ul.Name == "Izlenecekler" && ul.IsDefault),
  IsInReadList = userListsWithContent.Any(ul => ul.Name == "Okuduklarim" && ul.IsDefault),
         IsInToReadList = userListsWithContent.Any(ul => ul.Name == "Okunacaklar" && ul.IsDefault),
         CustomLists = userListsWithContent
  .Where(ul => !ul.IsDefault)
       .Select(ul => ul.Name)
 .ToList()
     };
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
      AverageRating = Math.Round(avgRating, 2),
    RatingsCount = ratingsCount,
  ReviewsCount = reviewsCount,
     ListAddCount = listAddCount,
      CurrentUserRating = currentUserRating,
        HasUserReviewed = hasUserReviewed,
          UserLibraryStatus = userLibraryStatus,
  Reviews = content.Reviews
      .OrderByDescending(r => r.CreatedAt)
          .Select(r => new ReviewDto
  {
     Id = r.Id,
   UserId = r.UserId,
     UserName = r.User.UserName,
         UserAvatarUrl = r.User.AvatarUrl,
      Text = r.Text,
         CreatedAt = r.CreatedAt
       })
  .ToList()
  };

    return Ok(ApiResponse<ContentDetailsDto>.SuccessResponse(dto, "Icerik detaylari basariyla getirildi."));
   }

        /// <summary>
        /// Yorum duzenleme (sadece kendi yorumunu duzenleyebilir)
        /// </summary>
        // PUT: api/content/review/{id}
        [HttpPut("review/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> UpdateReview(int id, [FromBody] ReviewContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.FailResponse(
                    "Gecersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound(ApiResponse.FailResponse("Yorum bulunamadi."));

            // Sadece kendi yorumunu düzenleyebilir
            if (review.UserId != userId)
                return Forbid();

            review.Text = model.Text;
        review.UpdatedAt = DateTime.UtcNow;

     _context.Reviews.Update(review);
   await _context.SaveChangesAsync();

    // Aktiviteyi de güncelle (varsa)
        var activity = await _context.Activities
    .FirstOrDefaultAsync(a => a.UserId == userId &&
  a.ContentId == review.ContentId &&
       a.ActivityType == "review");

   if (activity != null)
       {
   activity.Text = model.Text;
_context.Activities.Update(activity);
    await _context.SaveChangesAsync();
   }

 return Ok(ApiResponse.SuccessResponse("Yorum basariyla guncellendi."));
        }

        /// <summary>
        /// Yorum silme (sadece kendi yorumunu silebilir)
        /// </summary>
        // DELETE: api/content/review/{id}
   [HttpDelete("review/{id:int}")]
  [Authorize]
      public async Task<ActionResult<ApiResponse>> DeleteReview(int id)
      {
    var userId = GetCurrentUserId();

var review = await _context.Reviews
   .FirstOrDefaultAsync(r => r.Id == id);

if (review == null)
       return NotFound(ApiResponse.FailResponse("Yorum bulunamadi."));

       if (review.UserId != userId)
   return Forbid();

      _context.Reviews.Remove(review);

  var activity = await _context.Activities
    .FirstOrDefaultAsync(a => a.UserId == userId &&
     a.ContentId == review.ContentId &&
  a.ActivityType == "review");

if (activity != null)
      {
    _context.Activities.Remove(activity);
  }

   await _context.SaveChangesAsync();

return Ok(ApiResponse.SuccessResponse("Yorum basariyla silindi."));
    }

   /// <summary>
      /// Kullanicinin bir iceriğe verdigi puani getir
        /// </summary>
    [HttpGet("{contentId:int}/my-rating")]
      [Authorize]
  public async Task<ActionResult<ApiResponse<object>>> GetMyRating(int contentId)
        {
      var userId = GetCurrentUserId();

 var rating = await _context.Ratings
 .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);

 if (rating == null)
         return NotFound(ApiResponse<object>.FailResponse("Bu iceriğe henuz puan vermediniz."));

     return Ok(ApiResponse<object>.SuccessResponse(new
        {
     score = rating.Score,
   createdAt = rating.CreatedAt,
      updatedAt = rating.UpdatedAt
   }, "Puaniniz basariyla getirildi."));
   }

  /// <summary>
        /// Kullanicinin bir icerik hakkindaki yorumunu getir
  /// </summary>
        [HttpGet("{contentId:int}/my-review")]
        [Authorize]
  public async Task<ActionResult<ApiResponse<object>>> GetMyReview(int contentId)
  {
  var userId = GetCurrentUserId();

  var review = await _context.Reviews
   .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);

     if (review == null)
       return NotFound(ApiResponse<object>.FailResponse("Bu icerik hakkinda henuz yorum yapmadiniz."));

 return Ok(ApiResponse<object>.SuccessResponse(new
 {
      id = review.Id,
text = review.Text,
    createdAt = review.CreatedAt,
    updatedAt = review.UpdatedAt
   }, "Yorumunuz basariyla getirildi."));
 }
    }
}
