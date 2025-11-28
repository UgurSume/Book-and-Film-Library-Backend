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
        /// Aktiviteyi begen
        /// </summary>
        [HttpPost("{activityId:int}/begen")]
        public async Task<ActionResult<ApiResponse>> LikeActivity(int activityId)
        {
            var userId = GetCurrentUserId();

            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null)
                return NotFound(ApiResponse.FailResponse("Aktivite bulunamadi."));

            // Zaten beðenmiþ mi?
            var existingLike = await _context.ActivityLikes
                .FirstOrDefaultAsync(al => al.ActivityId == activityId && al.UserId == userId);

            if (existingLike != null)
                return BadRequest(ApiResponse.FailResponse("Bu aktiviteyi zaten begendiniz."));

            var like = new ActivityLike
            {
                ActivityId = activityId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLikes.Add(like);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Aktivite begenildi."));
        }

        /// <summary>
        /// Aktivite begenisini geri al
        /// </summary>
        [HttpDelete("{activityId:int}/begeniyi-geri-al")]
        public async Task<ActionResult<ApiResponse>> UnlikeActivity(int activityId)
        {
            var userId = GetCurrentUserId();

            var like = await _context.ActivityLikes
                .FirstOrDefaultAsync(al => al.ActivityId == activityId && al.UserId == userId);

            if (like == null)
                return NotFound(ApiResponse.FailResponse("Bu aktiviteyi begenmemissiniz."));

            _context.ActivityLikes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Begeni geri alindi."));
        }

        /// <summary>
        /// Aktiviteye yorum yap
        /// </summary>
        [HttpPost("{activityId:int}/yorum")]
        public async Task<ActionResult<ApiResponse<object>>> CommentOnActivity(
            int activityId,
            [FromBody] AddActivityCommentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Gecersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null)
                return NotFound(ApiResponse<object>.FailResponse("Aktivite bulunamadi."));

            var comment = new ActivityComment
            {
                ActivityId = activityId,
                UserId = userId,
                Text = model.Text,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityComments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                commentId = comment.Id
            }, "Yorum basariyla eklendi."));
        }

        /// <summary>
        /// Aktivitedeki yorumlari getir
        /// </summary>
        [HttpGet("{activityId:int}/yorumlar")]
        public async Task<ActionResult<ApiResponse<PagedResult<ActivityCommentDto>>>> GetActivityComments(
            int activityId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

            var userId = GetCurrentUserId();

            var totalCount = await _context.ActivityComments
                .Where(ac => ac.ActivityId == activityId)
                .CountAsync();

            var comments = await _context.ActivityComments
                .Where(ac => ac.ActivityId == activityId)
                .Include(ac => ac.User)
                .OrderBy(ac => ac.CreatedAt) // Eskiden yeniye
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            var pagedResult = new PagedResult<ActivityCommentDto>(result, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ActivityCommentDto>>.SuccessResponse(
                pagedResult, "Yorumlar basariyla getirildi."));
        }

        /// <summary>
        /// Yorumu guncelle (sadece kendi yorumu)
        /// </summary>
        [HttpPut("yorum/{commentId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateComment(
            int commentId,
            [FromBody] AddActivityCommentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.FailResponse(
                    "Gecersiz veri",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var userId = GetCurrentUserId();

            var comment = await _context.ActivityComments
                .FirstOrDefaultAsync(ac => ac.Id == commentId);

            if (comment == null)
                return NotFound(ApiResponse.FailResponse("Yorum bulunamadý."));

            if (comment.UserId != userId)
                return Forbid();

            comment.Text = model.Text;
            comment.UpdatedAt = DateTime.UtcNow;

            _context.ActivityComments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Yorum basariyla guncellendi."));
        }

        /// <summary>
        /// Yorumu sil (sadece kendi yorumu)
        /// </summary>
        [HttpDelete("yorum/{commentId:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();

            var comment = await _context.ActivityComments
                .FirstOrDefaultAsync(ac => ac.Id == commentId);

            if (comment == null)
                return NotFound(ApiResponse.FailResponse("Yorum bulunamadi."));

            if (comment.UserId != userId)
                return Forbid();

            _context.ActivityComments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Yorum basariyla silindi."));
        }

        /// <summary>
        /// Aktiviteyi begenen kullanicilari getir
        /// </summary>
        [HttpGet("{activityId:int}/begeniler")]
        public async Task<ActionResult<ApiResponse<PagedResult<FollowUserDto>>>> GetActivityLikes(
            int activityId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

            var userId = GetCurrentUserId();

            var totalCount = await _context.ActivityLikes
                .Where(al => al.ActivityId == activityId)
                .CountAsync();

            var likes = await _context.ActivityLikes
                .Where(al => al.ActivityId == activityId)
                .Include(al => al.User)
                .OrderByDescending(al => al.CreatedAt) // Yeniden eskiye
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            var pagedResult = new PagedResult<FollowUserDto>(result, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<FollowUserDto>>.SuccessResponse(
                pagedResult, "Begeniler basariyla getirildi."));
        }
    }
}
