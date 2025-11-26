using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Genel feed: tüm kullanıcıların son aktiviteleri
        // GET: api/feed?take=20
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> GetFeed([FromQuery] int take = 20)
        {
            if (take <= 0 || take > 100) take = 20;

            var activities = await _context.Activities
                .Include(a => a.User)
                .Include(a => a.Content)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();

            // List adlarını lookup olarak çekelim
            var listIds = activities
                .Where(a => a.ListId != null)
                .Select(a => a.ListId!.Value)
                .Distinct()
                .ToList();

            var lists = await _context.UserLists
                .Where(l => listIds.Contains(l.Id))
                .ToListAsync();

            var listNameById = lists.ToDictionary(l => l.Id, l => l.Name);

            var result = activities.Select(a => new ActivityDto
            {
                Id = a.Id,
                UserName = a.User.UserName,
                ContentTitle = a.Content.Title,
                ActivityType = a.ActivityType,
                Score = a.Score,
                Text = a.Text,
                ListName = a.ListId != null && listNameById.ContainsKey(a.ListId.Value)
                    ? listNameById[a.ListId.Value]
                    : null,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Ok(result);
        }

        // Belirli bir kullanıcının feed'i
        // GET: api/feed/user/3?take=20
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<List<ActivityDto>>> GetUserFeed(int userId, [FromQuery] int take = 20)
        {
            if (take <= 0 || take > 100) take = 20;

            var activities = await _context.Activities
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Content)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();

            var listIds = activities
                .Where(a => a.ListId != null)
                .Select(a => a.ListId!.Value)
                .Distinct()
                .ToList();

            var lists = await _context.UserLists
                .Where(l => listIds.Contains(l.Id))
                .ToListAsync();

            var listNameById = lists.ToDictionary(l => l.Id, l => l.Name);

            var result = activities.Select(a => new ActivityDto
            {
                Id = a.Id,
                UserName = a.User.UserName,
                ContentTitle = a.Content.Title,
                ActivityType = a.ActivityType,
                Score = a.Score,
                Text = a.Text,
                ListName = a.ListId != null && listNameById.ContainsKey(a.ListId.Value)
                    ? listNameById[a.ListId.Value]
                    : null,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Ok(result);
        }
    }
}
