using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1) İçeriği veritabanında garantiye al
        // POST: api/content/ensure
        [HttpPost("ensure")]
        public async Task<IActionResult> EnsureContent([FromBody] EnsureContentRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Contents
                .FirstOrDefaultAsync(c => c.ExternalId == model.ExternalId && c.Type == model.Type);

            if (existing != null)
                return Ok(existing);

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

            return Ok(content);
        }

        // 2) Puan verme (varsa güncelle, yoksa ekle)
        // POST: api/content/rate
        [HttpPost("rate")]
        public async Task<IActionResult> RateContent([FromBody] RateContentRequest model)
        {
            if (model.Score < 1 || model.Score > 10)
                return BadRequest("Puan 1 ile 10 arasında olmalı.");

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var content = await _context.Contents.FindAsync(model.ContentId);
            if (content == null)
                return NotFound("İçerik bulunamadı.");

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == model.UserId && r.ContentId == model.ContentId);

            if (rating == null)
            {
                rating = new Rating
                {
                    UserId = model.UserId,
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

            // Puan kaydını kaydedelim
            await _context.SaveChangesAsync();

            // Aktivite kaydı oluştur
            var activity = new Activity
            {
                UserId = model.UserId,
                ContentId = model.ContentId,
                ActivityType = "rating",
                Score = model.Score,
                CreatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Puan kaydedildi.",
                ratingId = rating.Id
            });
        }


        // 3) Yorum ekleme
        // POST: api/content/review
        [HttpPost("review")]
        public async Task<IActionResult> AddReview([FromBody] ReviewContentRequest model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var content = await _context.Contents.FindAsync(model.ContentId);
            if (content == null)
                return NotFound("İçerik bulunamadı.");

            if (string.IsNullOrWhiteSpace(model.Text))
                return BadRequest("Yorum metni boş olamaz.");

            var review = new Review
            {
                UserId = model.UserId,
                ContentId = model.ContentId,
                Text = model.Text,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Yorum kaydedildi.",
                reviewId = review.Id
            });
        }

        // 4) İçerik detay + ortalama puan + yorumlar
        // GET: api/content/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetContentDetails(int id)
        {
            var content = await _context.Contents
                .Include(c => c.Ratings)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
                return NotFound("İçerik bulunamadı.");

            double avgRating = 0;
            int ratingsCount = content.Ratings.Count;

            if (ratingsCount > 0)
            {
                avgRating = content.Ratings.Average(r => r.Score);
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
                AverageRating = avgRating,
                RatingsCount = ratingsCount,
                Reviews = content.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        UserName = r.User.UserName,
                        Text = r.Text,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList()
            };

            return Ok(dto);
        }
    }
}
