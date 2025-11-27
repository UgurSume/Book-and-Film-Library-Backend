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

        // 1) İçeriği veritabanında garantiye al
        // POST: api/content/ensure
        [HttpPost("ensure")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<Content>>> EnsureContent([FromBody] EnsureContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Content>.FailResponse(
                    "Geçersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var existing = await _context.Contents
                .FirstOrDefaultAsync(c => c.ExternalId == model.ExternalId && c.Type == model.Type);

            if (existing != null)
            {
                return Ok(ApiResponse<Content>.SuccessResponse(existing, "İçerik zaten mevcut."));
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

            return Ok(ApiResponse<Content>.SuccessResponse(content, "İçerik başarıyla eklendi."));
        }

        // 2) Puan verme (varsa güncelle, yoksa ekle)
        // POST: api/content/rate
        [HttpPost("rate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> RateContent([FromBody] RateContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Geçersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var content = await _context.Contents.FindAsync(model.ContentId);
            if (content == null)
                return NotFound(ApiResponse<object>.FailResponse("İçerik bulunamadı."));

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
            }, isNewRating ? "Puan başarıyla kaydedildi." : "Puan güncellendi."));
        }


        // 3) Yorum ekleme
        // POST: api/content/review
        [HttpPost("review")]
    [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> AddReview([FromBody] ReviewContentRequest model)
  {
    if (!ModelState.IsValid)
     {
   return BadRequest(ApiResponse<object>.FailResponse(
      "Geçersiz veri",
     ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
   }

   var userId = GetCurrentUserId();

   var content = await _context.Contents.FindAsync(model.ContentId);
      if (content == null)
     return NotFound(ApiResponse<object>.FailResponse("İçerik bulunamadı."));

    // Kullanıcı daha önce yorum yapmış mı?
  var existingReview = await _context.Reviews
.FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == model.ContentId);

if (existingReview != null)
  {
    return BadRequest(ApiResponse<object>.FailResponse(
       "Bu içerik hakkında zaten yorum yaptınız. Yorumunuzu düzenleyebilirsiniz."));
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

 // Aktivite kaydı oluştur
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
     }, "Yorum başarıyla kaydedildi."));
      }

        // 4) İçerik detay + ortalama puan + yorumlar + kullanıcı durumu
        // GET: api/content/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ContentDetailsDto>>> GetContentDetails(int id)
        {
      var content = await _context.Contents
                .Include(c => c.Ratings)
          .Include(c => c.Reviews)
         .ThenInclude(r => r.User)
    .FirstOrDefaultAsync(c => c.Id == id);

     if (content == null)
     return NotFound(ApiResponse<ContentDetailsDto>.FailResponse("İçerik bulunamadı."));

            double avgRating = 0;
      int ratingsCount = content.Ratings.Count;
   int reviewsCount = content.Reviews.Count;

            if (ratingsCount > 0)
     {
            avgRating = content.Ratings.Average(r => r.Score);
            }

            // Kaç kullanıcı listeye eklemiş
     int listAddCount = await _context.UserListItems
    .Where(uli => uli.ContentId == id)
             .CountAsync();

         // Giriş yapmış kullanıcının durumu (opsiyonel)
            int? currentUserRating = null;
         bool hasUserReviewed = false;
        UserLibraryStatusDto? userLibraryStatus = null;

 if (User.Identity?.IsAuthenticated == true)
            {
                var userId = GetCurrentUserId();

    // Kullanıcının puanı
    var userRating = await _context.Ratings
           .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == id);
     currentUserRating = userRating?.Score;

     // Kullanıcının yorumu var mı?
          hasUserReviewed = await _context.Reviews
         .AnyAsync(r => r.UserId == userId && r.ContentId == id);

   // Kullanıcının listelerindeki durumu
                var userLists = await _context.UserLists
  .Where(ul => ul.UserId == userId)
   .Include(ul => ul.Items)
           .ToListAsync();

     var userListsWithContent = userLists
 .Where(ul => ul.Items.Any(item => item.ContentId == id))
       .ToList();

    userLibraryStatus = new UserLibraryStatusDto
       {
   IsInWatchedList = userListsWithContent.Any(ul => ul.Name == "İzlediklerim" && ul.IsDefault),
         IsInToWatchList = userListsWithContent.Any(ul => ul.Name == "İzlenecekler" && ul.IsDefault),
       IsInReadList = userListsWithContent.Any(ul => ul.Name == "Okuduklarım" && ul.IsDefault),
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

    return Ok(ApiResponse<ContentDetailsDto>.SuccessResponse(dto, "İçerik detayları başarıyla getirildi."));
        }

        // 5) Yorum düzenleme (sadece kendi yorumunu düzenleyebilir)
        // PUT: api/content/review/{id}
        [HttpPut("review/{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> UpdateReview(int id, [FromBody] ReviewContentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.FailResponse(
                    "Geçersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound(ApiResponse.FailResponse("Yorum bulunamadı."));

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

 return Ok(ApiResponse.SuccessResponse("Yorum başarıyla güncellendi."));
        }

        // 6) Yorum silme (sadece kendi yorumunu silebilir)
        // DELETE: api/content/review/{id}
   [HttpDelete("review/{id:int}")]
  [Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteReview(int id)
      {
     var userId = GetCurrentUserId();

var review = await _context.Reviews
   .FirstOrDefaultAsync(r => r.Id == id);

if (review == null)
       return NotFound(ApiResponse.FailResponse("Yorum bulunamadı."));

   // Sadece kendi yorumunu silebilir
       if (review.UserId != userId)
   return Forbid();

      _context.Reviews.Remove(review);

// İlgili aktiviteyi de sil
  var activity = await _context.Activities
    .FirstOrDefaultAsync(a => a.UserId == userId &&
     a.ContentId == review.ContentId &&
  a.ActivityType == "review");

if (activity != null)
      {
    _context.Activities.Remove(activity);
  }

   await _context.SaveChangesAsync();

return Ok(ApiResponse.SuccessResponse("Yorum başarıyla silindi."));
        }

        // 7) Kullanıcının bir içeriğe verdiği puanı getir
    // GET: api/content/{contentId}/my-rating
    [HttpGet("{contentId:int}/my-rating")]
      [Authorize]
  public async Task<ActionResult<ApiResponse<object>>> GetMyRating(int contentId)
        {
      var userId = GetCurrentUserId();

 var rating = await _context.Ratings
 .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);

 if (rating == null)
         return NotFound(ApiResponse<object>.FailResponse("Bu içeriğe henüz puan vermediniz."));

     return Ok(ApiResponse<object>.SuccessResponse(new
        {
     score = rating.Score,
   createdAt = rating.CreatedAt,
      updatedAt = rating.UpdatedAt
   }, "Puanınız başarıyla getirildi."));
        }

  // 8) Kullanıcının bir içerik hakkındaki yorumunu getir
    // GET: api/content/{contentId}/my-review
        [HttpGet("{contentId:int}/my-review")]
        [Authorize]
  public async Task<ActionResult<ApiResponse<object>>> GetMyReview(int contentId)
        {
  var userId = GetCurrentUserId();

  var review = await _context.Reviews
   .FirstOrDefaultAsync(r => r.UserId == userId && r.ContentId == contentId);

     if (review == null)
       return NotFound(ApiResponse<object>.FailResponse("Bu içerik hakkında henüz yorum yapmadınız."));

    return Ok(ApiResponse<object>.SuccessResponse(new
 {
      id = review.Id,
text = review.Text,
    createdAt = review.CreatedAt,
    updatedAt = review.UpdatedAt
   }, "Yorumunuz başarıyla getirildi."));
 }
    }
}
