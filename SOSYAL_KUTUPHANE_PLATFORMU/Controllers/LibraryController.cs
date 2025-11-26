using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1) Kullanıcının tüm listelerini getir
        // GET: api/library/lists/1
        [HttpGet("lists/{userId:int}")]
        public async Task<ActionResult<List<UserListDto>>> GetUserLists(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return NotFound("Kullanıcı bulunamadı.");

            var lists = await _context.UserLists
                .Where(l => l.UserId == userId)
                .Include(l => l.Items)
                    .ThenInclude(i => i.Content)
                .ToListAsync();

            var result = lists.Select(l => new UserListDto
            {
                Id = l.Id,
                Name = l.Name,
                Type = l.Type,
                IsDefault = l.IsDefault,
                Items = l.Items.Select(i => new ListItemDto
                {
                    ContentId = i.ContentId,
                    Title = i.Content.Title,
                    Year = i.Content.Year,
                    CoverUrl = i.Content.CoverUrl,
                    Type = i.Content.Type
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // 2) Listeye içerik ekle
        // POST: api/library/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToList([FromBody] AddToListRequest model)
        {
            var list = await _context.UserLists
                .FirstOrDefaultAsync(l => l.Id == model.ListId && l.UserId == model.UserId);

            if (list == null)
                return NotFound("Liste bulunamadı veya bu kullanıcıya ait değil.");

            var content = await _context.Contents.FindAsync(model.ContentId);
            if (content == null)
                return NotFound("İçerik bulunamadı.");

            // Aynı içerik önceden eklenmiş mi?
            var exists = await _context.UserListItems
                .AnyAsync(i => i.UserListId == model.ListId && i.ContentId == model.ContentId);

            if (exists)
                return BadRequest("Bu içerik zaten listede mevcut.");

            var item = new UserListItem
            {
                UserListId = model.ListId,
                ContentId = model.ContentId
            };

            _context.UserListItems.Add(item);
            await _context.SaveChangesAsync();

            // Aktivite kaydı
            var activity = new Activity
            {
                UserId = list.UserId,
                ContentId = model.ContentId,
                ActivityType = "add_to_list",
                ListId = model.ListId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İçerik listeye eklendi." });

        }

        // 3) Listeden içerik sil
        // DELETE: api/library/remove
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromList([FromBody] RemoveFromListRequest model)
        {
            var list = await _context.UserLists
                .FirstOrDefaultAsync(l => l.Id == model.ListId && l.UserId == model.UserId);

            if (list == null)
                return NotFound("Liste bulunamadı veya bu kullanıcıya ait değil.");

            var item = await _context.UserListItems
                .FirstOrDefaultAsync(i => i.UserListId == model.ListId && i.ContentId == model.ContentId);

            if (item == null)
                return NotFound("Bu içerik listede bulunamadı.");

            _context.UserListItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İçerik listeden kaldırıldı." });
        }
    }
}
