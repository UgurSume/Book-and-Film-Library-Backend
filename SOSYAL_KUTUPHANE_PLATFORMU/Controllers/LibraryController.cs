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
    public class LibraryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1) Kullanıcının tüm listelerini getir
        // GET: api/library/lists/{userId}
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

        // GET: api/library/my-lists (Kendi listelerini getir)
        [HttpGet("my-lists")]
        public async Task<ActionResult<List<UserListDto>>> GetMyLists()
        {
            var userId = GetCurrentUserId();

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
            var userId = GetCurrentUserId(); // Token'dan al

            var list = await _context.UserLists
                .FirstOrDefaultAsync(l => l.Id == model.ListId && l.UserId == userId);

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
                UserId = userId,
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
            var userId = GetCurrentUserId(); // Token'dan al

            var list = await _context.UserLists
                .FirstOrDefaultAsync(l => l.Id == model.ListId && l.UserId == userId);

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

        // YENİ: Özel liste oluştur
        // POST: api/library/create-list
        [HttpPost("create-list")]
     public async Task<IActionResult> CreateList([FromBody] CreateListRequest model)
  {
        var userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(model.Name))
     return BadRequest("Liste adı boş olamaz.");

            // Aynı isimde liste var mı kontrol et
     var exists = await _context.UserLists
     .AnyAsync(l => l.UserId == userId && l.Name == model.Name);

     if (exists)
       return BadRequest("Bu isimde bir liste zaten mevcut.");

          var list = new UserList
 {
        UserId = userId,
                Name = model.Name,
       Type = model.Type ?? "mixed", // movie, book veya mixed
         IsDefault = false,
       CreatedAt = DateTime.UtcNow
       };

         _context.UserLists.Add(list);
       await _context.SaveChangesAsync();

            return Ok(new
            {
    message = "Liste oluşturuldu.",
      listId = list.Id,
    name = list.Name
    });
   }

        // YENİ: Özel liste güncelle
        // PUT: api/library/update-list/{listId}
    [HttpPut("update-list/{listId:int}")]
        public async Task<IActionResult> UpdateList(int listId, [FromBody] CreateListRequest model)
{
       var userId = GetCurrentUserId();

            var list = await _context.UserLists
      .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId);

            if (list == null)
     return NotFound("Liste bulunamadı veya bu kullanıcıya ait değil.");

        if (list.IsDefault)
  return BadRequest("Varsayılan listeler düzenlenemez.");

       if (string.IsNullOrWhiteSpace(model.Name))
 return BadRequest("Liste adı boş olamaz.");

 // Aynı isimde başka liste var mı kontrol et
            var exists = await _context.UserLists
     .AnyAsync(l => l.UserId == userId && l.Name == model.Name && l.Id != listId);

    if (exists)
    return BadRequest("Bu isimde bir liste zaten mevcut.");

      list.Name = model.Name;
       if (!string.IsNullOrEmpty(model.Type))
        list.Type = model.Type;

          _context.UserLists.Update(list);
         await _context.SaveChangesAsync();

            return Ok(new { message = "Liste güncellendi." });
        }

      // YENİ: Özel liste sil
        // DELETE: api/library/delete-list/{listId}
     [HttpDelete("delete-list/{listId:int}")]
        public async Task<IActionResult> DeleteList(int listId)
     {
       var userId = GetCurrentUserId();

   var list = await _context.UserLists
                .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == listId && l.UserId == userId);

  if (list == null)
    return NotFound("Liste bulunamadı veya bu kullanıcıya ait değil.");

  if (list.IsDefault)
return BadRequest("Varsayılan listeler silinemez.");

  // Liste öğelerini sil
   _context.UserListItems.RemoveRange(list.Items);

   // İlgili aktiviteleri sil
    var activities = await _context.Activities
          .Where(a => a.ListId == listId)
  .ToListAsync();

    _context.Activities.RemoveRange(activities);

    // Listeyi sil
            _context.UserLists.Remove(list);

            await _context.SaveChangesAsync();

return Ok(new { message = "Liste silindi." });
      }
    }
}
