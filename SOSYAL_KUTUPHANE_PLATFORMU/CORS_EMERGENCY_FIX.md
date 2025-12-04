# ACÝL: CORS Hatasý Çözümü

## ? HATA:
```
Access to XMLHttpRequest at 'https://localhost:7297/api/Content/1180831/my-rating' 
from origin 'http://localhost:3001' has been blocked by CORS policy: 
No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

---

## ? ÇÖZÜM: BACKEND'Ý YENÝDEN BAÞLAT!

### Visual Studio'da:
```
1. Shift + F5 (Durdur)
2. F5 (Baþlat)
3. Swagger açýlana kadar bekle: https://localhost:7297/swagger
4. Frontend'i yenile (Ctrl + F5)
```

### Terminal'de:
```sh
# Backend duruyorsa Ctrl + C ile durdur
cd C:\Users\ugurs\OneDrive\Masaüstü\Yazlab\SOSYAL_KUTUPHANE_PLATFORMU\SOSYAL_KUTUPHANE_PLATFORMU
dotnet run
```

---

## ?? YAPILAN DEÐÝÞÝKLÝKLER

### 1. Program.cs - CORS Güçlendirildi:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
  "http://localhost:3000",
   "http://localhost:3001",  // ? FRONTEND PORT
   "http://localhost:5173",
  "http://localhost:4200"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
  .WithExposedHeaders("*"); // ? YENÝ: Tüm header'larý expose et
    });
});

// ? YENÝ: URL'leri küçük harfe çevir
builder.Services.AddRouting(options => 
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});
```

### 2. ContentController - Route Çoklandý:
```csharp
[ApiController]
[Route("api/[controller]")]  // Content (case-insensitive)
[Route("api/Content")]        // ? Büyük C
[Route("api/content")]   // ? Küçük c
public class ContentController : BaseController
```

**Artýk þu URL'lerin HEPSÝ çalýþýr:**
- ? `/api/Content/1/reviews`
- ? `/api/content/1/reviews`
- ? `/api/CONTENT/1/reviews`

---

## ?? TEST ADIMLARI

### 1. Backend Çalýþýyor mu Kontrol:
```sh
# Terminal çýktýsýnda þunu görmeli:
info: Microsoft.Hosting.Lifetime[14]
   Now listening on: https://localhost:7297
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 2. Swagger'da Test:
```
URL: https://localhost:7297/swagger

Endpoint'leri kontrol et:
? GET /api/content/{contentId}/reviews
? GET /api/content/{contentId}/my-rating
```

### 3. Browser Console'da CORS Testi:
Frontend'de F12 ? Console:
```javascript
// Test 1: Reviews (Token gereksiz)
fetch('https://localhost:7297/api/Content/1/reviews?page=1')
  .then(res => res.json())
  .then(data => console.log('? CORS çalýþýyor!', data))
  .catch(err => console.error('? CORS hatasý:', err));

// Test 2: My Rating (Token gerekli)
const token = localStorage.getItem('authToken');
fetch('https://localhost:7297/api/Content/1/my-rating', {
  headers: { 'Authorization': `Bearer ${token}` }
})
  .then(res => res.json())
  .then(data => console.log('? Token çalýþýyor!', data))
  .catch(err => console.error('? Token hatasý:', err));
```

---

## ?? SORUN TESPÝT

### ? CORS Baþarýlý Response Headers:
Browser Network tab'de (F12 ? Network) þunlarý görmeli:

```http
access-control-allow-origin: http://localhost:3001
access-control-allow-credentials: true
access-control-allow-methods: GET, POST, PUT, DELETE
access-control-allow-headers: *
```

### ? CORS Baþarýsýz:
Eðer hala CORS hatasý alýyorsanýz:

1. **Backend yeniden baþlatýldý mý?** ? Mutlaka yeniden baþlat!
2. **Frontend port'u doðru mu?** ? `http://localhost:3001` olmalý
3. **Browser cache temiz mi?** ? Ctrl + Shift + Delete, cache temizle
4. **Incognito mode'da dene** ? Cache/cookie sorunu olmasýn

---

## ?? HIZLI ÇÖZÜM KONTROL LÝSTESÝ

- [ ] Backend'i durdur (Shift + F5 veya Ctrl + C)
- [ ] Backend'i baþlat (F5 veya `dotnet run`)
- [ ] Swagger açýlana kadar bekle
- [ ] Frontend'i yenile (Ctrl + F5 - hard reload)
- [ ] Browser console'da CORS hatasý var mý kontrol et
- [ ] Network tab'de response headers'ý kontrol et

---

## ?? SIK KARÞILAÞILAN DURUMLAR

### Durum 1: Backend Çalýþýyor Ama CORS Hatasý Devam Ediyor

**Sebep:** Backend kod deðiþikliði yaptýktan sonra yeniden baþlatýlmadý.

**Çözüm:**
```sh
# Visual Studio'da:
Shift + F5 ? F5

# Terminal'de:
Ctrl + C ? dotnet run
```

---

### Durum 2: "No 'Access-Control-Allow-Origin' header"

**Sebep:** CORS middleware düzgün yüklenmedi veya sýralama yanlýþ.

**Kontrol:**
```csharp
// Program.cs'de sýralama:
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");  // ? Authentication'dan ÖNCE
app.UseAuthentication();
app.UseAuthorization();
```

---

### Durum 3: OPTIONS Ýsteði 404

**Sebep:** Preflight request'e backend cevap veremiyor.

**Kontrol:**
```csharp
// CORS policy'de .AllowAnyMethod() olmalý
policy.AllowAnyMethod()  // ? OPTIONS dahil tüm HTTP metodlarý
```

---

### Durum 4: Credentials True Ama Hala Hata

**Sebep:** Frontend'de `withCredentials: true` eksik olabilir.

**Frontend (axios):**
```javascript
axios.defaults.withCredentials = true;

// VEYA her istekte:
axios.get('/api/content/1/reviews', {
  withCredentials: true
});
```

---

## ?? SON KONTROL

### Backend Log'larýný Ýzle:
Visual Studio ? View ? Output ? Show output from: "Debug"

**Görmek istediðiniz:**
```
info: Microsoft.AspNetCore.Cors.Infrastructure.CorsService[0]
      CORS policy execution successful.
```

### Frontend Network Tab:
F12 ? Network ? Ýstek seç ? Headers

**Request Headers:**
```
Origin: http://localhost:3001
```

**Response Headers:**
```
access-control-allow-origin: http://localhost:3001 ?
```

---

## ?? DÝKKAT!

### CORS Deðiþiklikleri Sonrasý MUTLAKA:
1. ? Backend'i yeniden baþlat
2. ? Frontend'i hard reload yap (Ctrl + F5)
3. ? Browser cache temizle
4. ? Incognito mode'da test et

### CORS Çalýþmazsa:
1. Backend'in çalýþtýðýndan emin ol
2. Swagger'ýn açýldýðýný doðrula
3. Frontend port'unun CORS listesinde olduðunu kontrol et
4. Browser console'da tam hata mesajýný oku
5. Network tab'de preflight request'i (OPTIONS) kontrol et

---

## ?? ACÝL DESTEK

### Backend Çalýþmýyor:
```sh
# Port dinlenip dinlenmediðini kontrol et:
netstat -ano | findstr :7297

# Process kill (Windows):
taskkill /PID <PID> /F

# Yeniden baþlat:
dotnet run
```

### Frontend Baðlanamýyor:
```javascript
// Axios config'i kontrol et:
console.log('Base URL:', axios.defaults.baseURL);
// Beklenen: https://localhost:7297/api

console.log('With Credentials:', axios.defaults.withCredentials);
// Beklenen: true (eðer cookie kullanýyorsanýz)
```

---

Son Güncelleme: 2024
Durum: ? CORS GÜÇLENDIRILDI - ROUTE ÇOKLANDI - BACKEND YENÝDEN BAÞLATILMALI!

**ÖNEMLÝ: Bu deðiþiklikler sadece backend yeniden baþlatýlýnca etkili olur!**
