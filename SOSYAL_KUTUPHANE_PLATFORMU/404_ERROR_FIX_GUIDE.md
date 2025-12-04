# 404 Hata Çözümü - Content Endpoint'leri

## ? Sorun:
Frontend'den þu URL'lere istek atýlýyor ama 404 alýnýyor:
```
GET /api/Content/1180831/my-rating  ? 404
GET /api/Content/1180831/reviews?page=1 ? 404
```

---

## ? ÇÖZÜM UYGULANDI

### 1. Route Deðiþiklikleri:
```csharp
[ApiController]
[Route("api/[controller]")]  // Content (case-insensitive)
[Route("api/content")]    // Açýkça küçük harf
public class ContentController : BaseController
```

### 2. Endpoint'lere Logging Eklendi:
```csharp
[HttpGet("{contentId:int}/reviews")]
public async Task<ActionResult> GetContentReviewsPaginated(int contentId, [FromQuery] int page = 1)
{
    _logger.LogInformation($"GetContentReviews called for ContentId: {contentId}, Page: {page}");
    // ...
}

[HttpGet("{contentId:int}/my-rating")]
[Authorize]
public async Task<ActionResult<ApiResponse<object>>> GetMyRating(int contentId)
{
    var userId = GetCurrentUserId();
    _logger.LogInformation($"GetMyRating called for User: {userId}, ContentId: {contentId}");
    // ...
}
```

---

## ?? TEST ADIMLARI

### 1. Backend'i Yeniden Baþlatýn:
```sh
# Visual Studio'da:
Shift + F5 (Durdur)
F5 (Baþlat)

# VEYA Terminal'de:
cd SOSYAL_KUTUPHANE_PLATFORMU
dotnet run
```

### 2. Swagger'da Manuel Test:
```
URL: https://localhost:7297/swagger

Test edilecek endpoint'ler:
? GET /api/content/{contentId}/reviews?page=1
? GET /api/content/{contentId}/my-rating (Token gerekli)
```

### 3. Browser Console'da Direct Test:
Frontend olmadan test etmek için browser console'da (F12):

```javascript
// Test 1: Reviews (Token gereksiz)
fetch('https://localhost:7297/api/content/1/reviews?page=1')
  .then(res => res.json())
  .then(data => console.log('Reviews:', data))
  .catch(err => console.error('Error:', err));

// Test 2: My Rating (Token gerekli)
const token = localStorage.getItem('authToken'); // Frontend'den token alýn
fetch('https://localhost:7297/api/content/1/my-rating', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
  .then(res => res.json())
  .then(data => console.log('My Rating:', data))
  .catch(err => console.error('Error:', err));
```

---

## ?? SORUN TESPÝT KONTROL LÝSTESÝ

### ? Backend Kontrolü:

1. **Backend Çalýþýyor mu?**
   ```sh
   # Terminal'de þu çýktýyý görmelisiniz:
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: https://localhost:7297
   ```

2. **Endpoint'ler Swagger'da Görünüyor mu?**
   - `https://localhost:7297/swagger` açýn
   - `Content` grubunda þu endpoint'leri görmeli:
     - `GET /api/content/{contentId}/reviews`
     - `GET /api/content/{contentId}/my-rating`

3. **Veritabanýnda Content Var mý?**
   ```sql
   -- SSMS'de çalýþtýrýn:
   SELECT TOP 10 * FROM Contents ORDER BY Id DESC
   
   -- Örnek Content ID: 1, 2, 3...
   -- Frontend'in kullandýðý ID'nin var olmasý gerekli!
   ```

### ? Frontend Kontrolü:

1. **Frontend Doðru URL Kullanýyor mu?**
   ```javascript
   // ? YANLIÞ:
   axios.get(`/api/Content/${contentId}/reviews`) // Büyük C
   
   // ? DOÐRU (ama büyük C de artýk destekleniyor):
   axios.get(`/api/content/${contentId}/reviews`) // Küçük c
   ```

2. **baseURL Doðru Ayarlanmýþ mý?**
   ```javascript
   // axios config'de:
   axios.defaults.baseURL = 'https://localhost:7297/api';
   
   // O zaman:
   axios.get(`/content/${contentId}/reviews`) // /api otomatik eklenir
   ```

3. **contentId Geçerli mi?**
   ```javascript
   // Frontend'de:
   console.log('ContentId:', contentId); // 1180831 gibi bir sayý
   
   // Backend'de bu ID veritabanýnda olmalý!
   ```

### ? CORS Kontrolü:

1. **Browser Console'da CORS Hatasý Var mý?**
   ```
   ? Access to XMLHttpRequest at 'https://localhost:7297/api/content/1/reviews' 
      from origin 'http://localhost:3001' has been blocked by CORS policy
   ```

   **Çözüm:** `Program.cs`'de CORS zaten ayarlý ama frontend port'u kontrol edin:
   ```csharp
   policy.WithOrigins(
       "http://localhost:3000",
       "http://localhost:3001",  // ? Frontend port'unuz bu mu?
       "http://localhost:5173"
   )
   ```

---

## ?? BACKEND LOG'LARINI KONTROL EDIN

### Visual Studio Output Window:
```
# Ýyi log örneði (istek geldi):
info: ContentController[0]
      GetContentReviews called for ContentId: 1180831, Page: 1

# Hata log örneði (404):
warn: ContentController[0]
      Content not found with ID: 1180831
```

### Eðer Log Görmüyorsanýz:
Ýstek backend'e **hiç ulaþmýyor** demektir. Þunlarý kontrol edin:
- Backend çalýþýyor mu?
- Frontend doðru URL'i kullanýyor mu?
- CORS engeli var mý?

---

## ??? ÇÖZÜM SENARYOLARI

### Senaryo 1: ContentId Veritabanýnda Yok
**Hata:** `Content not found with ID: 1180831`

**Çözüm:**
```javascript
// Frontend'de önce içeriði ensure edin:
const ensureContent = async (movieData) => {
  const response = await axios.post('/content/ensure', {
    externalId: movieData.id.toString(), // TMDb ID
    type: 'movie',
    title: movieData.title,
    description: movieData.overview,
    year: movieData.release_date ? parseInt(movieData.release_date.substring(0, 4)) : null,
    coverUrl: movieData.poster_path ? 
      `https://image.tmdb.org/t/p/w500${movieData.poster_path}` : null
  }, {
    headers: {
    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
    }
  });
  
  return response.data.data.contentId; // Backend'deki ID
};

// Sonra bu ID ile iþlem yapýn
const backendContentId = await ensureContent(movieData);
axios.get(`/content/${backendContentId}/reviews`);
```

---

### Senaryo 2: Token Geçersiz (401 Unauthorized)
**Hata:** `GET /api/content/1/my-rating ? 401 Unauthorized`

**Çözüm:**
```javascript
// Token'ýn varlýðýný kontrol edin:
const token = localStorage.getItem('authToken');
if (!token) {
  console.error('Token yok, login gerekli');
  window.location.href = '/login';
  return;
}

// Token'ýn süresi dolmuþ olabilir:
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
  localStorage.removeItem('authToken');
   window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
```

---

### Senaryo 3: HTTPS Sertifika Hatasý
**Hata:** `net::ERR_CERT_AUTHORITY_INVALID`

**Çözüm:**
```sh
# Development certificate'i güvenilir yap:
dotnet dev-certs https --trust

# Alternatif: HTTP kullan (güvenli deðil, sadece development için)
# appsettings.Development.json:
{
  "Urls": "http://localhost:5000"
}
```

---

## ?? SON KONTROL LÝSTESÝ

Backend'i yeniden baþlattýktan sonra:

- [ ] Backend çalýþýyor (`https://localhost:7297`)
- [ ] Swagger açýlýyor ve endpoint'ler görünüyor
- [ ] Browser console'da CORS hatasý yok
- [ ] Frontend doðru base URL kullanýyor
- [ ] ContentId veritabanýnda var
- [ ] Token geçerli (my-rating için)
- [ ] Backend log'larýnda istek görünüyor

---

## ?? TEST KOMUTLARý

### cURL ile Test:
```sh
# Test 1: Reviews (Token gereksiz)
curl -X GET "https://localhost:7297/api/content/1/reviews?page=1" -k

# Test 2: My Rating (Token gerekli)
curl -X GET "https://localhost:7297/api/content/1/my-rating" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -k

# Test 3: Content Ensure (Token gerekli)
curl -X POST "https://localhost:7297/api/content/ensure" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"externalId":"550","type":"movie","title":"Fight Club","year":1999}' \
  -k
```

### PowerShell ile Test:
```powershell
# Test 1: Reviews
Invoke-RestMethod -Uri "https://localhost:7297/api/content/1/reviews?page=1" -Method Get

# Test 2: My Rating (Token ile)
$token = "YOUR_TOKEN_HERE"
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri "https://localhost:7297/api/content/1/my-rating" -Method Get -Headers $headers
```

---

## ?? SON ÖNERÝLER

1. **Backend'i Yeniden Baþlatýn** - Deðiþiklikler etkili olsun
2. **Browser Cache'i Temizleyin** - Ctrl + Shift + Delete
3. **Incognito Mode'da Deneyin** - Cache/Cookie problemi olmasýn
4. **Backend Log'larýný Ýzleyin** - Visual Studio Output window
5. **Network Tab'i Açýk Tutun** - F12 > Network, istekleri görün

---

Son Güncelleme: 2024
Durum: ? ROUTE'LAR DÜZELTÝLDÝ - LOGGING EKLENDÝ - TEST REHBERÝ HAZIR
