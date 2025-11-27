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

     // GET: api/user/profile/{userId}
        [HttpGet("profile/{userId:int}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(int userId)
    {
  var currentUserId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Kullanýcý bulunamadý.");

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
       IsOwnProfile = currentUserId == userId
      };

            return Ok(dto);
   }

    // GET: api/user/my-profile
        [HttpGet("my-profile")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
      var userId = GetCurrentUserId();
     return await GetUserProfile(userId);
        }

        // PUT: api/user/update-profile
     [HttpPut("update-profile")]
 public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest model)
 {
            var userId = GetCurrentUserId();

   var user = await _context.Users.FindAsync(userId);
      if (user == null)
        return NotFound("Kullanýcý bulunamadý.");

  // Sadece dolu alanlarý güncelle
            if (model.Biography != null)
   user.Biography = model.Biography;

            if (model.AvatarUrl != null)
  user.AvatarUrl = model.AvatarUrl;

            _context.Users.Update(user);
       await _context.SaveChangesAsync();

         return Ok(new { message = "Profil güncellendi." });
        }

        // POST: api/user/follow/{userId}
  [HttpPost("follow/{userId:int}")]
  public async Task<IActionResult> FollowUser(int userId)
        {
            var currentUserId = GetCurrentUserId();

    if (currentUserId == userId)
         return BadRequest("Kendinizi takip edemezsiniz.");

    var userToFollow = await _context.Users.FindAsync(userId);
            if (userToFollow == null)
          return NotFound("Kullanýcý bulunamadý.");

          // Zaten takip ediyor mu?
            var existingFollow = await _context.UserFollowers
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == userId);

            if (existingFollow != null)
     return BadRequest("Bu kullanýcýyý zaten takip ediyorsunuz.");

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

    return Ok(new { message = "Kullanýcý takip edildi." });
    }

        // DELETE: api/user/unfollow/{userId}
    [HttpDelete("unfollow/{userId:int}")]
   public async Task<IActionResult> UnfollowUser(int userId)
      {
     var currentUserId = GetCurrentUserId();

     if (currentUserId == userId)
          return BadRequest("Kendinizi takipten çýkaramazsýnýz.");

            var follow = await _context.UserFollowers
  .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == userId);

            if (follow == null)
     return NotFound("Bu kullanýcýyý takip etmiyorsunuz.");

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

         return Ok(new { message = "Kullanýcý takipten çýkarýldý." });
     }

        // GET: api/user/{userId}/followers
        [HttpGet("{userId:int}/followers")]
 public async Task<ActionResult<List<FollowUserDto>>> GetFollowers(int userId)
        {
 var currentUserId = GetCurrentUserId();

     var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
  if (!userExists)
    return NotFound("Kullanýcý bulunamadý.");

            var followers = await _context.UserFollowers
              .Where(uf => uf.FollowingId == userId)
      .Include(uf => uf.Follower)
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

         return Ok(result);
      }

        // GET: api/user/{userId}/following
        [HttpGet("{userId:int}/following")]
     public async Task<ActionResult<List<FollowUserDto>>> GetFollowing(int userId)
        {
         var currentUserId = GetCurrentUserId();

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
  if (!userExists)
       return NotFound("Kullanýcý bulunamadý.");

     var following = await _context.UserFollowers
       .Where(uf => uf.FollowerId == userId)
         .Include(uf => uf.Following)
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

        return Ok(result);
  }

        // GET: api/user/my-followers
  [HttpGet("my-followers")]
   public async Task<ActionResult<List<FollowUserDto>>> GetMyFollowers()
        {
            var userId = GetCurrentUserId();
       return await GetFollowers(userId);
        }

        // GET: api/user/my-following
     [HttpGet("my-following")]
        public async Task<ActionResult<List<FollowUserDto>>> GetMyFollowing()
        {
            var userId = GetCurrentUserId();
   return await GetFollowing(userId);
        }
    }
}
