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
    public class ActivityController : BaseController
    {
   private readonly ApplicationDbContext _context;

    public ActivityController(ApplicationDbContext context)
 {
            _context = context;
        }

    /// <summary>
     /// Aktiviteyi beðen
        /// POST: api/activity/{activityId}/like
        /// </summary>
        [HttpPost("{activityId:int}/like")]
        public async Task<IActionResult> LikeActivity(int activityId)
   {
     var userId = GetCurrentUserId();

    var activity = await _context.Activities.FindAsync(activityId);
       if (activity == null)
      return NotFound("Aktivite bulunamadý.");

          // Zaten beðenmiþ mi?
 var existingLike = await _context.ActivityLikes
      .FirstOrDefaultAsync(al => al.ActivityId == activityId && al.UserId == userId);

            if (existingLike != null)
     return BadRequest("Bu aktiviteyi zaten beðendiniz.");

            var like = new ActivityLike
  {
   ActivityId = activityId,
     UserId = userId,
      CreatedAt = DateTime.UtcNow
   };

            _context.ActivityLikes.Add(like);
       await _context.SaveChangesAsync();

         return Ok(new { message = "Aktivite beðenildi." });
        }

      /// <summary>
     /// Aktivite beðenisini geri al
      /// DELETE: api/activity/{activityId}/unlike
        /// </summary>
    [HttpDelete("{activityId:int}/unlike")]
        public async Task<IActionResult> UnlikeActivity(int activityId)
        {
  var userId = GetCurrentUserId();

            var like = await _context.ActivityLikes
        .FirstOrDefaultAsync(al => al.ActivityId == activityId && al.UserId == userId);

 if (like == null)
       return NotFound("Bu aktiviteyi beðenmemiþsiniz.");

   _context.ActivityLikes.Remove(like);
     await _context.SaveChangesAsync();

            return Ok(new { message = "Beðeni geri alýndý." });
      }

        /// <summary>
        /// Aktiviteye yorum yap
     /// POST: api/activity/{activityId}/comment
        /// </summary>
  [HttpPost("{activityId:int}/comment")]
        public async Task<IActionResult> CommentOnActivity(
      int activityId, 
   [FromBody] AddActivityCommentRequest model)
        {
            if (!ModelState.IsValid)
    return BadRequest(ModelState);

var userId = GetCurrentUserId();

         var activity = await _context.Activities.FindAsync(activityId);
  if (activity == null)
      return NotFound("Aktivite bulunamadý.");

            var comment = new ActivityComment
          {
    ActivityId = activityId,
  UserId = userId,
       Text = model.Text,
   CreatedAt = DateTime.UtcNow
   };

   _context.ActivityComments.Add(comment);
       await _context.SaveChangesAsync();

       return Ok(new
  {
       message = "Yorum eklendi.",
    commentId = comment.Id
       });
        }

        /// <summary>
   /// Aktivitedeki yorumlarý getir
        /// GET: api/activity/{activityId}/comments?skip=0&take=20
   /// </summary>
  [HttpGet("{activityId:int}/comments")]
        public async Task<ActionResult<List<ActivityCommentDto>>> GetActivityComments(
   int activityId,
       [FromQuery] int skip = 0,
      [FromQuery] int take = 20)
        {
       if (take <= 0 || take > 100) take = 20;
            if (skip < 0) skip = 0;

var userId = GetCurrentUserId();

   var comments = await _context.ActivityComments
   .Where(ac => ac.ActivityId == activityId)
       .Include(ac => ac.User)
                .OrderBy(ac => ac.CreatedAt) // Eskiden yeniye
    .Skip(skip)
        .Take(take)
      .ToListAsync();

  var result = comments.Select(c => new ActivityCommentDto
   {
 Id = c.Id,
    ActivityId = c.ActivityId,
         UserId = c.UserId,
     UserName = c.User.UserName,
      UserAvatarUrl = c.User.AvatarUrl,
       Text = c.Text,
             CreatedAt = c.CreatedAt,
     UpdatedAt = c.UpdatedAt,
     IsOwnComment = c.UserId == userId
      }).ToList();

   return Ok(result);
        }

        /// <summary>
        /// Yorumu güncelle (sadece kendi yorumu)
  /// PUT: api/activity/comment/{commentId}
        /// </summary>
        [HttpPut("comment/{commentId:int}")]
   public async Task<IActionResult> UpdateComment(
   int commentId,
      [FromBody] AddActivityCommentRequest model)
        {
            if (!ModelState.IsValid)
      return BadRequest(ModelState);

       var userId = GetCurrentUserId();

         var comment = await _context.ActivityComments
  .FirstOrDefaultAsync(ac => ac.Id == commentId);

            if (comment == null)
        return NotFound("Yorum bulunamadý.");

     if (comment.UserId != userId)
          return Forbid();

comment.Text = model.Text;
            comment.UpdatedAt = DateTime.UtcNow;

    _context.ActivityComments.Update(comment);
       await _context.SaveChangesAsync();

     return Ok(new { message = "Yorum güncellendi." });
   }

        /// <summary>
        /// Yorumu sil (sadece kendi yorumu)
 /// DELETE: api/activity/comment/{commentId}
        /// </summary>
   [HttpDelete("comment/{commentId:int}")]
   public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();

var comment = await _context.ActivityComments
  .FirstOrDefaultAsync(ac => ac.Id == commentId);

          if (comment == null)
      return NotFound("Yorum bulunamadý.");

            if (comment.UserId != userId)
   return Forbid();

  _context.ActivityComments.Remove(comment);
         await _context.SaveChangesAsync();

       return Ok(new { message = "Yorum silindi." });
  }

        /// <summary>
        /// Aktiviteyi beðenen kullanýcýlarý getir
     /// GET: api/activity/{activityId}/likes?skip=0&take=20
  /// </summary>
        [HttpGet("{activityId:int}/likes")]
        public async Task<ActionResult<List<FollowUserDto>>> GetActivityLikes(
      int activityId,
   [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
   if (take <= 0 || take > 100) take = 20;
  if (skip < 0) skip = 0;

     var userId = GetCurrentUserId();

     var likes = await _context.ActivityLikes
     .Where(al => al.ActivityId == activityId)
       .Include(al => al.User)
  .OrderByDescending(al => al.CreatedAt) // Yeniden eskiye
        .Skip(skip)
      .Take(take)
        .ToListAsync();

    // Giriþ yapan kullanýcýnýn takip ettiði kiþileri al
    var currentUserFollowingIds = await _context.UserFollowers
  .Where(uf => uf.FollowerId == userId)
       .Select(uf => uf.FollowingId)
            .ToListAsync();

   var result = likes.Select(l => new FollowUserDto
            {
      Id = l.User.Id,
      UserName = l.User.UserName,
   AvatarUrl = l.User.AvatarUrl,
    Biography = l.User.Biography,
      FollowersCount = l.User.FollowersCount,
     FollowingCount = l.User.FollowingCount,
                IsFollowing = currentUserFollowingIds.Contains(l.User.Id)
      }).ToList();

            return Ok(result);
        }
    }
}
