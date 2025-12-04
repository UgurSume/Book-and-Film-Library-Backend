# ?? Sosyal Kütüphane Platformu

## ?? Proje Hakkýnda
Film ve kitap paylaþým platformu. Kullanýcýlar içerikleri puanlayabilir, yorum yapabilir ve listelere ekleyebilir.

## ?? Teknolojiler
- **.NET 8** - Backend
- **Entity Framework Core 8** - ORM
- **SQL Server** - Veritabaný
- **JWT** - Authentication
- **TMDb API** - Film verileri
- **Google Books API** - Kitap verileri

## ?? Kurulum

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

## ?? API Endpoints

### Authentication
- `POST /api/Auth/register` - Kayýt ol
- `POST /api/Auth/login` - Giriþ yap

### Content
- `POST /api/Content/ensure` - Ýçerik ekle
- `POST /api/Content/rate` - Puan ver
- `POST /api/Content/review` - Yorum yaz
- `GET /api/Content/{id}` - Ýçerik detayý

### Library
- `GET /api/Library/my-lists` - Listelerim
- `POST /api/Library/add` - Listeye ekle

### User
- `GET /api/User/my-profile` - Profilim
- `POST /api/User/follow/{userId}` - Takip et

### Feed
- `GET /api/Feed` - Ana akýþ

## ?? Yapýlandýrma

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

## ?? Modeller

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

## ?? Test

### Swagger ile Test
1. `/api/Auth/login` ile token al
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

## ?? Dokümantasyon

Detaylý dokümantasyon için `Docs/` klasörüne bakýn.

## ?? Katkýda Bulunma

1. Fork edin
2. Feature branch oluþturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add some amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açýn

## ?? Lisans

Bu proje MIT lisansý altýndadýr.

## ?? Geliþtirici

**Uður Süme**
- GitHub: [@UgurSume](https://github.com/UgurSume)

## ?? Ýletiþim

Sorularýnýz için:
- GitHub Issues: [Issues](https://github.com/UgurSume/SOSYAL_KUTUPHANE_PLATFORMU/issues)
- Email: [ugursume@example.com](mailto:ugursume@example.com)

---

**Son Güncelleme:** 2024-12-04  
**Versiyon:** 1.0.0
