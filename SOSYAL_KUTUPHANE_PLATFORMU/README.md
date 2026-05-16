#  Sosyal Kütüphane Platformu
# Sosyal Kütüphane Platformu

## Proje Hakkýnda
Film ve kitap paylaþým platformu. Kullanýcýlar içerikleri puanlayabilir, yorum yapabilir ve listelere ekleyebilir.

## Teknolojiler
- **.NET 8** - Backend
- **Entity Framework Core 8** - ORM
- **SQL Server** - Veritabaný
- **JWT** - Authentication
- **TMDb API** - Film verileri
- **Google Books API** - Kitap verileri

## Kurulum

### 1. Repository'yi Klonla
```bash
git clone https://github.com/UgurSume/SOSYAL_KUTUPHANE_PLATFORMU
cd SOSYAL_KUTUPHANE_PLATFORMU
```

### 2. appsettings.json'u Yapýlandýr
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SosyalKutuphaneDb;..."
  },
  "Jwt": {
    "SecretKey": "YOUR_SECRET_KEY_HERE",
    "Issuer": "SosyalKutuphane",
    "Audience": "SosyalKutuphaneUsers"
  },
  "ExternalApis": {
    "Tmdb": {
      "ApiKey": "YOUR_TMDB_API_KEY"
    }
  }
}
```

### 3. Database Migration
```bash
dotnet ef database update
```

### 4. Projeyi Çalýþtýr
```bash
dotnet run
```

Swagger: `https://localhost:7297/swagger`

## API Endpoints

### Authentication
- `POST /api/Auth/kayit` - Kayýt ol
- `POST /api/Auth/giris` - Giriþ yap
- `GET /api/Auth/me` - Mevcut kullanýcý

### Content
- `POST /api/Content/ensure` - Ýçerik ekle
- `POST /api/Content/rate` - Puan ver
- `POST /api/Content/review` - Yorum yaz
- `GET /api/Content/{id}` - Ýçerik detayý
- `GET /api/Content/{id}/average-rating` - Ortalama kullanýcý puaný
- `GET /api/Content/external/{externalId}/average-rating` - ExternalId ile puan

### Library
- `GET /api/Library/my-lists` - Listelerim
- `POST /api/Library/add` - Listeye ekle
- `POST /api/Library/add-by-status` - Duruma göre ekle (watched/to_watch/read/to_read)
- `DELETE /api/Library/remove` - Listeden çýkar

### User
- `GET /api/User/my-profile` - Profilim
- `GET /api/User/profile/{id}` - Kullanýcý profili
- `PUT /api/User/update-profile` - Profil güncelle
- `GET /api/User/search?query=` - Kullanýcý ara
- `GET /api/User/my-ratings` - Verdiðim puanlar
- `POST /api/User/follow/{userId}` - Takip et
- `DELETE /api/User/unfollow/{userId}` - Takibi býrak

### Search
- `GET /api/Search/movies?query=` - Film ara
- `GET /api/Search/books?query=` - Kitap ara
- `GET /api/Search/all?query=` - Hepsinde ara
- `POST /api/Search/filter` - Geliþmiþ filtreleme

### Discover
- `GET /api/Discover/top-rated` - En yüksek puanlýlar
- `GET /api/Discover/popular` - En popülerler
- `GET /api/Discover/trending` - Trendler
- `GET /api/Discover/recent` - Yeni eklenenler
- `GET /api/Discover/recommended` - Önerilen içerikler

### Feed
- `GET /api/Feed` - Ana akýþ
- `GET /api/Feed/my` - Kendi aktivitelerim
- `GET /api/Feed/explore` - Tüm platform

---

### CORS
Frontend için CORS yapýlandýrmasý `Program.cs`'de tanýmlýdýr:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
 .AllowAnyHeader()
           .AllowCredentials();
 });
});
```

### JWT
Token süresi: 1440 dakika (24 saat)

## Modeller

### User
- UserName, Email, PasswordHash
- FollowersCount, FollowingCount
- Biography, AvatarUrl

### Content
- ExternalId (TMDb/Google Books ID)
- Type (movie/book)
- Title, Description, Year, CoverUrl
- Director, Cast, Genres (film için)
- Authors, PageCount (kitap için)

### Activity
- UserId, ContentId
- ActivityType (rating/review/add_to_list)
- Score, Text, ListId

## Test

### Swagger ile Test
1. `/api/Auth/giris` ile token al
2. Swagger'da "Authorize" butonuna týkla
3. `Bearer {token}` formatýnda yapýþtýr
4. Endpoint'leri test et

### SQL Kontrolleri
```sql
-- Aktiviteler
SELECT * FROM Activities ORDER BY CreatedAt DESC;

-- Ýçerikler
SELECT * FROM Contents;

-- Kullanýcýlar
SELECT * FROM Users;
```












---


