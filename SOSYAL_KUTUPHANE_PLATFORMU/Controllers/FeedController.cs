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
    [Authorize]
    public class FeedController : BaseController
 {
   private readonly ApplicationDbContext _context;

        public FeedController(ApplicationDbContext context)
      {
            _context = context;
        }

        /// <summary>
        /// Takip edilen kullanıcıların aktivitelerini gösterir (Ana Feed)
        /// GET: api/feed?pageNumber=1&pageSize=15
      /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetFeed(
       [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 15)
    {
            var currentUserId = GetCurrentUserId();

        // Validation
          if (pageSize <= 0 || pageSize > 50) pageSize = 15;
            if (pageNumber <= 0) pageNumber = 1;

        // Takip edilen kullanıcıların ID'lerini al
            var followingIds = await _context.UserFollowers
             .Where(uf => uf.FollowerId == currentUserId)
    .Select(uf => uf.FollowingId)
          .ToListAsync();

            // Eğer kimseyi takip etmiyorsa boş sonuç dön
if (!followingIds.Any())
  {
        var emptyResult = new PagedResult<ActivityDto>(
     new List<ActivityDto>(), 0, pageNumber, pageSize);
   return Ok(ApiResponse<PagedResult<ActivityDto>>.SuccessResponse(
             emptyResult, "Henüz kimseyi takip etmiyorsunuz."));
 }

     // Toplam aktivite sayısı
  var totalCount = await _context.Activities
  .Where(a => followingIds.Contains(a.UserId))
           .CountAsync();

   // Aktiviteleri getir
          var activities = await _context.Activities
  .Where(a => followingIds.Contains(a.UserId))
              .Include(a => a.User)
.Include(a => a.Content)
    .Include(a => a.Likes)
            .Include(a => a.Comments)
    .OrderByDescending(a => a.CreatedAt)
              .Skip((pageNumber - 1) * pageSize)
   .Take(pageSize)
.ToListAsync();

            var activityDtos = await MapToActivityDtos(activities, currentUserId);
      var pagedResult = new PagedResult<ActivityDto>(
    activityDtos, totalCount, pageNumber, pageSize);

         return Ok(ApiResponse<PagedResult<ActivityDto>>.SuccessResponse(
            pagedResult, "Feed başarıyla getirildi."));
   }

        /// <summary>
     /// Belirli bir kullanıcının aktivite geçmişi
   /// GET: api/feed/user/3?pageNumber=1&pageSize=15
        /// </summary>
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetUserFeed(
   int userId,
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 15)
        {
            var currentUserId = GetCurrentUserId();

      // Validation
            if (pageSize <= 0 || pageSize > 50) pageSize = 15;
  if (pageNumber <= 0) pageNumber = 1;

     // Kullanıcı var mı kontrolü
          var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
         return NotFound(ApiResponse<PagedResult<ActivityDto>>.FailResponse(
        "Kullanıcı bulunamadı."));
 }

            // Toplam aktivite sayısı
      var totalCount = await _context.Activities
    .Where(a => a.UserId == userId)
          .CountAsync();

         // Aktiviteleri getir
         var activities = await _context.Activities
       .Where(a => a.UserId == userId)
    .Include(a => a.User)
      .Include(a => a.Content)
       .Include(a => a.Likes)
     .Include(a => a.Comments)
                .OrderByDescending(a => a.CreatedAt)
       .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
      .ToListAsync();

            var activityDtos = await MapToActivityDtos(activities, currentUserId);
            var pagedResult = new PagedResult<ActivityDto>(
    activityDtos, totalCount, pageNumber, pageSize);

            return Ok(ApiResponse<PagedResult<ActivityDto>>.SuccessResponse(
          pagedResult, "Kullanıcı aktiviteleri başarıyla getirildi."));
        }

 /// <summary>
    /// Kendi aktivite geçmişinizi getirir
        /// GET: api/feed/my?pageNumber=1?pageSize=15
     /// </summary>
  [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetMyFeed(
       [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 15)
        {
      var userId = GetCurrentUserId();
     return await GetUserFeed(userId, pageNumber, pageSize);
        }

        /// <summary>
    /// Keşfet feed'i - Tüm platformdaki aktiviteler
        /// GET: api/feed/explore?pageNumber=1?pageSize=15
        /// </summary>
        [HttpGet("explore")]
      public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetExploreFeed(
        [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 15)
    {
            var currentUserId = GetCurrentUserId();

   // Validation
            if (pageSize <= 0 || pageSize > 50) pageSize = 15;
          if (pageNumber <= 0) pageNumber = 1;

   // Toplam aktivite sayısı
  var totalCount = await _context.Activities.CountAsync();

       // Aktiviteleri getir
            var activities = await _context.Activities
        .Include(a => a.User)
     .Include(a => a.Content)
       .Include(a => a.Likes)
           .Include(a => a.Comments)
    .OrderByDescending(a => a.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
  .Take(pageSize)
      .ToListAsync();

            var activityDtos = await MapToActivityDtos(activities, currentUserId);
      var pagedResult = new PagedResult<ActivityDto>(
     activityDtos, totalCount, pageNumber, pageSize);

return Ok(ApiResponse<PagedResult<ActivityDto>>.SuccessResponse(
      pagedResult, "Keşfet feed'i başarıyla getirildi."));
        }

        // Helper method: Activities'i ActivityDto'ya map et
        private async Task<List<ActivityDto>> MapToActivityDtos(List<Models.Activity> activities, int currentUserId)
        {
            // Liste adlarını lookup olarak çekelim
            var listIds = activities
     .Where(a => a.ListId != null)
      .Select(a => a.ListId!.Value)
      .Distinct()
             .ToList();

       var lists = await _context.UserLists
            .Where(l => listIds.Contains(l.Id))
  .ToListAsync();

          var listNameById = lists.ToDictionary(l => l.Id, l => l.Name);

            // Kullanıcının beğendiği aktiviteleri al
  var likedActivityIds = await _context.ActivityLikes
            .Where(al => al.UserId == currentUserId)
            .Select(al => al.ActivityId)
                .ToListAsync();

         var result = activities.Select(a =>
            {
        const int ExcerptLength = 180; // Proje metninde 150-200 karakter
         var hasMoreText = false;
     string? textExcerpt = null;

       // Review aktivitesi için excerpt oluştur
  if (a.ActivityType == "review" && !string.IsNullOrEmpty(a.Text))
            {
  hasMoreText = a.Text.Length > ExcerptLength;
      textExcerpt = hasMoreText 
         ? a.Text.Substring(0, ExcerptLength) + "..." 
             : a.Text;
    }

          return new ActivityDto
      {
          Id = a.Id,
        UserId = a.UserId,
      UserName = a.User.UserName,
      UserAvatarUrl = a.User.AvatarUrl,
 ContentId = a.ContentId,
   ContentTitle = a.Content.Title,
  ContentCoverUrl = a.Content.CoverUrl,
         ContentType = a.Content.Type,
           ActivityType = a.ActivityType,
         Score = a.Score,
         Text = a.Text,
        TextExcerpt = textExcerpt,
   HasMoreText = hasMoreText,
         ListName = a.ListId != null && listNameById.ContainsKey(a.ListId.Value)
  ? listNameById[a.ListId.Value]
   : null,
     CreatedAt = a.CreatedAt,
     LikesCount = a.Likes.Count,
         CommentsCount = a.Comments.Count,
         IsLikedByCurrentUser = likedActivityIds.Contains(a.Id)
                };
   }).ToList();

 return result;
        }
    }
}
