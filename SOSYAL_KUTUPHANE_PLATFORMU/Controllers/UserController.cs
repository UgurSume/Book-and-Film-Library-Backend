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
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

   public UserController(ApplicationDbContext context)
        {
 _context = context;
        }

        /// <summary>
     /// Kullanýcý profilini getir
     /// GET: api/user/profile/{userId}
    /// </summary>
     [HttpGet("profile/{userId:int}")]
  public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUserProfile(int userId)
   {
     var currentUserId = GetCurrentUserId();

   var user = await _context.Users.FindAsync(userId);
    if (user == null)
   return NotFound(ApiResponse<UserProfileDto>.FailResponse("Kullanýcý bulunamadý."));

      // Kullanýcý istatistikleri
      var totalRatings = await _context.Ratings.CountAsync(r => r.UserId == userId);
    var totalReviews = await _context.Reviews.CountAsync(r => r.UserId == userId);
      var totalLists = await _context.UserLists.CountAsync(ul => ul.UserId == userId && !ul.IsDefault);
   var totalActivities = await _context.Activities.CountAsync(a => a.UserId == userId);

   // Giriþ yapan kullanýcý bu profili takip ediyor mu?
 var isFollowing = await _context.UserFollowers
 .AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == userId);

    var dto = new UserProfileDto
  {
     Id = user.Id,
    UserName = user.UserName,
   Email = user.Email,
       AvatarUrl = user.AvatarUrl,
      Biography = user.Biography,
 FollowersCount = user.FollowersCount,
FollowingCount = user.FollowingCount,
       CreatedAt = user.CreatedAt,
  IsFollowing = isFollowing,
          IsOwnProfile = currentUserId == userId,
  // Ýstatistikler
            TotalRatings = totalRatings,
     TotalReviews = totalReviews,
  TotalLists = totalLists,
            TotalActivities = totalActivities
  };

      return Ok(ApiResponse<UserProfileDto>.SuccessResponse(dto, "Profil baþarýyla getirildi."));
        }

   /// <summary>
     /// Kendi profilini getir
  /// GET: api/user/my-profile
   /// </summary>
      [HttpGet("my-profile")]
  public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetMyProfile()
     {
       var userId = GetCurrentUserId();
  return await GetUserProfile(userId);
   }

        /// <summary>
        /// Kullanýcý ara
    /// GET: api/user/search?query=ahmet
        /// </summary>
      [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<FollowUserDto>>>> SearchUsers([FromQuery] string query)
  {
      if (string.IsNullOrWhiteSpace(query))
      return BadRequest(ApiResponse<List<FollowUserDto>>.FailResponse("Arama sorgusu boþ olamaz."));

            var currentUserId = GetCurrentUserId();

            // Kullanýcýlarý ara
       var users = await _context.Users
      .Where(u => u.UserName.Contains(query) && u.Id != currentUserId) // Kendini hariç tut
   .Take(20)
       .ToListAsync();

            // Giriþ yapan kullanýcýnýn takip ettiði kiþileri al
      var currentUserFollowingIds = await _context.UserFollowers
           .Where(uf => uf.FollowerId == currentUserId)
     .Select(uf => uf.FollowingId)
    .ToListAsync();

         var result = users.Select(u => new FollowUserDto
         {
     Id = u.Id,
      UserName = u.UserName,
          AvatarUrl = u.AvatarUrl,
     Biography = u.Biography,
  FollowersCount = u.FollowersCount,
     FollowingCount = u.FollowingCount,
          IsFollowing = currentUserFollowingIds.Contains(u.Id)
      }).ToList();

    return Ok(ApiResponse<List<FollowUserDto>>.SuccessResponse(
        result,
  $"{result.Count} kullanýcý bulundu."));
        }

    /// <summary>
        /// Profil güncelleme
 /// PUT: api/user/update-profile
      /// </summary>
  [HttpPut("update-profile")]
        public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileRequest model)
        {
 if (!ModelState.IsValid)
{
         return BadRequest(ApiResponse.FailResponse(
 "Geçersiz veri",
          ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
          }

   var userId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);
        if (user == null)
    return NotFound(ApiResponse.FailResponse("Kullanýcý bulunamadý."));

   // Sadece dolu alanlarý güncelle
     if (!string.IsNullOrWhiteSpace(model.UserName))
  user.UserName = model.UserName;

    if (model.Biography != null)
  user.Biography = model.Biography;

     if (model.AvatarUrl != null)
       user.AvatarUrl = model.AvatarUrl;

       _context.Users.Update(user);
       await _context.SaveChangesAsync();

 return Ok(ApiResponse.SuccessResponse("Profil baþarýyla güncellendi."));
        }

  /// <summary>
        /// Kullanýcýyý takip et
   /// POST: api/user/follow/{userId}
        /// </summary>
        [HttpPost("follow/{userId:int}")]
        public async Task<ActionResult<ApiResponse>> FollowUser(int userId)
        {
     var currentUserId = GetCurrentUserId();

  if (currentUserId == userId)
    return BadRequest(ApiResponse.FailResponse("Kendinizi takip edemezsiniz."));

var userToFollow = await _context.Users.FindAsync(userId);
      if (userToFollow == null)
     return NotFound(ApiResponse.FailResponse("Kullanýcý bulunamadý."));

      // Zaten takip ediyor mu?
  var existingFollow = await _context.UserFollowers
         .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == userId);

       if (existingFollow != null)
    return BadRequest(ApiResponse.FailResponse("Bu kullanýcýyý zaten takip ediyorsunuz."));

            // Takip kaydý oluþtur
     var follow = new UserFollower
            {
   FollowerId = currentUserId,
  FollowingId = userId,
       CreatedAt = DateTime.UtcNow
     };

  _context.UserFollowers.Add(follow);

 // Sayaçlarý güncelle
var currentUser = await _context.Users.FindAsync(currentUserId);
   if (currentUser != null)
       {
   currentUser.FollowingCount++;
    _context.Users.Update(currentUser);
     }

userToFollow.FollowersCount++;
       _context.Users.Update(userToFollow);

    await _context.SaveChangesAsync();

     return Ok(ApiResponse.SuccessResponse("Kullanýcý baþarýyla takip edildi."));
        }

      /// <summary>
     /// Kullanýcýyý takipten çýkar
      /// DELETE: api/user/unfollow/{userId}
   /// </summary>
        [HttpDelete("unfollow/{userId:int}")]
   public async Task<ActionResult<ApiResponse>> UnfollowUser(int userId)
        {
            var currentUserId = GetCurrentUserId();

if (currentUserId == userId)
          return BadRequest(ApiResponse.FailResponse("Kendinizi takipten çýkaramazsýnýz."));

 var follow = await _context.UserFollowers
      .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == userId);

     if (follow == null)
     return NotFound(ApiResponse.FailResponse("Bu kullanýcýyý takip etmiyorsunuz."));

  _context.UserFollowers.Remove(follow);

      // Sayaçlarý güncelle
  var currentUser = await _context.Users.FindAsync(currentUserId);
    if (currentUser != null && currentUser.FollowingCount > 0)
       {
   currentUser.FollowingCount--;
   _context.Users.Update(currentUser);
      }

     var userToUnfollow = await _context.Users.FindAsync(userId);
 if (userToUnfollow != null && userToUnfollow.FollowersCount > 0)
     {
        userToUnfollow.FollowersCount--;
 _context.Users.Update(userToUnfollow);
      }

      await _context.SaveChangesAsync();

       return Ok(ApiResponse.SuccessResponse("Kullanýcý takipten çýkarýldý."));
        }

        /// <summary>
   /// Kullanýcýnýn takipçilerini getir
 /// GET: api/user/{userId}/followers?pageNumber=1&pageSize=20
        /// </summary>
   [HttpGet("{userId:int}/followers")]
        public async Task<ActionResult<ApiResponse<PagedResult<FollowUserDto>>>> GetFollowers(
       int userId,
  [FromQuery] int pageNumber = 1,
   [FromQuery] int pageSize = 20)
   {
     if (pageSize <= 0 || pageSize > 100) pageSize = 20;
   if (pageNumber <= 0) pageNumber = 1;

       var currentUserId = GetCurrentUserId();

var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
  if (!userExists)
      return NotFound(ApiResponse<PagedResult<FollowUserDto>>.FailResponse("Kullanýcý bulunamadý."));

 var totalCount = await _context.UserFollowers
         .Where(uf => uf.FollowingId == userId)
        .CountAsync();

     var followers = await _context.UserFollowers
   .Where(uf => uf.FollowingId == userId)
     .Include(uf => uf.Follower)
          .OrderByDescending(uf => uf.CreatedAt)
.Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
     .Select(uf => uf.Follower)
        .ToListAsync();

      // Giriþ yapan kullanýcýnýn takip ettiði kiþileri al
            var currentUserFollowingIds = await _context.UserFollowers
       .Where(uf => uf.FollowerId == currentUserId)
   .Select(uf => uf.FollowingId)
    .ToListAsync();

  var result = followers.Select(u => new FollowUserDto
       {
    Id = u.Id,
UserName = u.UserName,
    AvatarUrl = u.AvatarUrl,
      Biography = u.Biography,
 FollowersCount = u.FollowersCount,
       FollowingCount = u.FollowingCount,
       IsFollowing = currentUserFollowingIds.Contains(u.Id)
      }).ToList();

    var pagedResult = new PagedResult<FollowUserDto>(result, totalCount, pageNumber, pageSize);
return Ok(ApiResponse<PagedResult<FollowUserDto>>.SuccessResponse(
pagedResult, "Takipçiler baþarýyla getirildi."));
        }

        /// <summary>
        /// Kullanýcýnýn takip ettiklerini getir
        /// GET: api/user/{userId}/following?pageNumber=1&pageSize=20
   /// </summary>
  [HttpGet("{userId:int}/following")]
        public async Task<ActionResult<ApiResponse<PagedResult<FollowUserDto>>>> GetFollowing(
int userId,
       [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 20)
        {
    if (pageSize <= 0 || pageSize > 100) pageSize = 20;
 if (pageNumber <= 0) pageNumber = 1;

       var currentUserId = GetCurrentUserId();

var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
  if (!userExists)
      return NotFound(ApiResponse<PagedResult<FollowUserDto>>.FailResponse("Kullanýcý bulunamadý."));

            var totalCount = await _context.UserFollowers
.Where(uf => uf.FollowerId == userId)
      .CountAsync();

 var following = await _context.UserFollowers
       .Where(uf => uf.FollowerId == userId)
    .Include(uf => uf.Following)
      .OrderByDescending(uf => uf.CreatedAt)
     .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
       .Select(uf => uf.Following)
                .ToListAsync();

// Giriþ yapan kullanýcýnýn takip ettiði kiþileri al
   var currentUserFollowingIds = await _context.UserFollowers
      .Where(uf => uf.FollowerId == currentUserId)
    .Select(uf => uf.FollowingId)
.ToListAsync();

   var result = following.Select(u => new FollowUserDto
 {
            Id = u.Id,
                UserName = u.UserName,
     AvatarUrl = u.AvatarUrl,
 Biography = u.Biography,
      FollowersCount = u.FollowersCount,
         FollowingCount = u.FollowingCount,
           IsFollowing = currentUserFollowingIds.Contains(u.Id)
  }).ToList();

var pagedResult = new PagedResult<FollowUserDto>(result, totalCount, pageNumber, pageSize);
    return Ok(ApiResponse<PagedResult<FollowUserDto>>.SuccessResponse(
   pagedResult, "Takip edilenler baþarýyla getirildi."));
     }

        /// <summary>
        /// Kendi takipçilerini getir
 /// GET: api/user/my-followers?pageNumber=1&pageSize=20
   /// </summary>
      [HttpGet("my-followers")]
   public async Task<ActionResult<ApiResponse<PagedResult<FollowUserDto>>>> GetMyFollowers(
    [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 20)
   {
   var userId = GetCurrentUserId();
     return await GetFollowers(userId, pageNumber, pageSize);
        }

     /// <summary>
 /// Kendi takip ettiklerini getir
 /// GET: api/user/my-following?pageNumber=1&pageSize=20
/// </summary>
        [HttpGet("my-following")]
   public async Task<ActionResult<ApiResponse<PagedResult<FollowUserDto>>>> GetMyFollowing(
   [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
   {
       var userId = GetCurrentUserId();
     return await GetFollowing(userId, pageNumber, pageSize);
 }
    }
}
