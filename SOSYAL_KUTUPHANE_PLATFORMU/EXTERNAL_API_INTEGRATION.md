# Harici API Entegrasyonu - Proje Metni Uyumlulugu

## Durum: ? TAMAMLANDI

Proje metni 2.2.1 maddesindeki tum gereksinimler karsilanmistir.

---

## ?? TMDb (Film) Entegrasyonu

### ? Cekilecek Veriler (Proje Metni):
| Gereksinim | Durum | Alan Adi |
|------------|-------|----------|
| Film basligi | ? | `Title` |
| Ozet | ? | `Description` (Overview) |
| Yayin yili | ? | `Year` (ReleaseDate) |
| **Yonetmen** | ? | `Director` |
| **Oyuncular** | ? | `Cast` (List<string>) |
| **Turler** | ? | `Genres` (List<string>) |
| Kapak gorseli URL'si | ? | `CoverUrl` (PosterPath) |

### API Endpoint'leri:
```csharp
// 1. Film arama
GET /search/movie?api_key={key}&query={query}&language=tr-TR

// 2. Film detaylari (turler icin)
GET /movie/{movieId}?api_key={key}&language=tr-TR

// 3. Film kadrosu (yonetmen ve oyuncular icin)
GET /movie/{movieId}/credits?api_key={key}&language=tr-TR
```

### Implementasyon:
- **Service:** `TmdbService.cs`
- **Metod:** `SearchMoviesAsync(string query)`
- **Detay Metod:** `GetMovieDetailsAsync(int movieId)` (private)

### Ornek Response:
```json
{
  "externalId": "550",
  "type": "movie",
  "title": "Fight Club",
  "description": "Bir ofis calisaninin hikayesi...",
  "year": 1999,
  "coverUrl": "https://image.tmdb.org/t/p/w500/abc123.jpg",
  "director": "David Fincher",
  "cast": ["Brad Pitt", "Edward Norton", "Helena Bonham Carter"],
  "genres": ["Drama", "Action"]
}
```

---

## ?? Google Books (Kitap) Entegrasyonu

### ? Cekilecek Veriler (Proje Metni):
| Gereksinim | Durum | Alan Adi |
|------------|-------|----------|
| Kitap basligi | ? | `Title` |
| **Yazar(lar)** | ? | `Authors` (List<string>) |
| Aciklama | ? | `Description` |
| **Sayfa sayisi** | ? | `PageCount` |
| Kapak gorseli URL'si | ? | `CoverUrl` (Thumbnail) |
| Yayin yili | ? | `Year` (PublishedDate) |

### API Endpoint'leri:
```csharp
// Kitap arama
GET /volumes?q={query}
```

### Implementasyon:
- **Service:** `GoogleBooksService.cs`
- **Metod:** `SearchBooksAsync(string query)`

### Ornek Response:
```json
{
  "externalId": "abc123xyz",
  "type": "book",
  "title": "Harry Potter ve Felsefe Tasi",
  "description": "Harry Potter'in macerasi...",
  "year": 1997,
  "coverUrl": "https://books.google.com/books/content?id=abc&printsec=frontcover&img=1",
  "authors": ["J.K. Rowling"],
  "pageCount": 223
}
```

---

## ??? Database Modeli

### Content Tablosu (Guncel):
```sql
CREATE TABLE Contents (
    Id INT PRIMARY KEY IDENTITY,
    ExternalId NVARCHAR(255) NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- 'movie' veya 'book'
    Title NVARCHAR(500) NOT NULL,
    Description NVARCHAR(MAX),
    Year INT,
    CoverUrl NVARCHAR(1000),
    
    -- FILM ICIN:
    Director NVARCHAR(500),       -- Yonetmen
    Cast NVARCHAR(MAX),        -- Oyuncular (JSON array)
    Genres NVARCHAR(MAX),         -- Turler (JSON array)
    
    -- KITAP ICIN:
    Authors NVARCHAR(MAX),        -- Yazarlar (JSON array)
    PageCount INT,       -- Sayfa sayisi
 
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## ?? DTO'lar

### SearchResultItemDto (Guncel):
```csharp
public class SearchResultItemDto
{
    public string ExternalId { get; set; }
    public string Type { get; set; } // "movie" veya "book"
    public string Title { get; set; }
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? CoverUrl { get; set; }
    
    // Film icin
    public string? Director { get; set; }
  public List<string>? Cast { get; set; }
    public List<string>? Genres { get; set; }
    
    // Kitap icin
    public List<string>? Authors { get; set; }
    public int? PageCount { get; set; }
}
```

---

## ?? API Kullanimi

### Frontend icin Ornek:

#### Film Arama:
```javascript
GET /api/search/movies?query=inception&pageNumber=1&pageSize=20

Response:
{
  "success": true,
  "message": "'inception' icin 15 film bulundu.",
  "data": {
    "items": [
    {
      "externalId": "27205",
        "type": "movie",
        "title": "Inception",
    "description": "Cobb bir hirsizdir...",
    "year": 2010,
        "coverUrl": "https://image.tmdb.org/t/p/w500/...",
     "director": "Christopher Nolan",
  "cast": ["Leonardo DiCaprio", "Marion Cotillard", "Tom Hardy"],
        "genres": ["Action", "Sci-Fi", "Thriller"]
      }
    ],
    "currentPage": 1,
    "totalCount": 15,
    "hasNext": false
  }
}
```

#### Kitap Arama:
```javascript
GET /api/search/books?query=harry+potter&pageNumber=1&pageSize=20

Response:
{
  "success": true,
  "message": "'harry potter' icin 40 kitap bulundu.",
  "data": {
    "items": [
      {
     "externalId": "abc123",
        "type": "book",
        "title": "Harry Potter ve Felsefe Tasi",
 "description": "Harry Potter bir gun...",
    "year": 1997,
        "coverUrl": "https://books.google.com/...",
        "authors": ["J.K. Rowling"],
        "pageCount": 223
      }
    ],
    "currentPage": 1,
    "totalCount": 40,
    "hasNext": true
  }
}
```

---

## ?? Konfigurasyonlar

### appsettings.json:
```json
{
  "ExternalApis": {
    "Tmdb": {
      "ApiKey": "YOUR_TMDB_API_KEY",
      "BaseUrl": "https://api.themoviedb.org/3",
      "ImageBaseUrl": "https://image.tmdb.org/t/p/w500"
  },
    "GoogleBooks": {
      "BaseUrl": "https://www.googleapis.com/books/v1"
    }
  }
}
```

### User Secrets (Gelistirme):
```bash
dotnet user-secrets set "ExternalApis:Tmdb:ApiKey" "your_api_key_here"
```

---

## ?? Proje Metni Karsilastirmasi

### ? Tum Gereksinimler Karsilandi:

| Proje Metni Gereksinimi | Implementasyon |
|--------------------------|----------------|
| "Platformdaki tum film ve kitap meta verileri, harici servisten cekilmelidir" | ? TMDb + Google Books entegrasyonu |
| "Manuel veri girisi yapilmayacaktir" | ? Tum veriler API'den cekilir |
| **FILMLER (TMDb):** |  |
| Film basligi | ? Title |
| Ozet | ? Description |
| Yayin yili | ? Year |
| Yonetmen | ? Director (Credits API) |
| Oyuncular | ? Cast (Credits API) |
| Turler | ? Genres (Detail API) |
| Kapak gorseli URL'si | ? CoverUrl |
| **KITAPLAR (Google Books):** |  |
| Kitap basligi | ? Title |
| Yazar(lar) | ? Authors |
| Aciklama | ? Description |
| Sayfa sayisi | ? PageCount |
| Kapak gorseli URL'si | ? CoverUrl |

---

## ?? Sonraki Adimlar

1. ? **Migration Uygulandi** - Yeni alanlar veritabanina eklendi
2. ? **API Servisleri Guncellendi** - Tum veriler cekiliyor
3. ? **DTO'lar Guncellendi** - Frontend icin hazir
4. ? **Test Edilmesi Gerekiyor** - API key'lerin calistigini dogrulayin

---

## ?? API Key'leri Alma:

### TMDb API Key:
1. https://www.themoviedb.org/signup adresinden kayit olun
2. Ayarlar > API > API Key olusturun
3. User Secrets'a ekleyin

### Google Books API:
- API Key gerekmez (rate limit vardir)
- Daha fazla istek icin: https://console.cloud.google.com/

---

Son Guncelleme: 2024
Durum: ? PROJE METNI GEREKSINIMLERINE TAM UYUMLU
