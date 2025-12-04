# CORS Sorunu Cozumu ve Test Rehberi

## ? CORS Ayari TAMAMLANDI

### Guncelleme Yapildi:
`Program.cs` dosyasinda CORS policy guncellendi.

### Eklenen Frontend Port'lari:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
         "http://localhost:3000",  // React (Create React App)
     "http://localhost:3001",  // React (Custom Port) ? SÝZÝN FRONTEND
"http://localhost:5173",  // Vite
            "http://localhost:4200",  // Angular
            "http://localhost:8080"   // Vue
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

### CORS Middleware Sirasi (DOÐRU):
```csharp
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");    // ? Authentication'dan ÖNCE
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## ?? Backend'i Yeniden Baslatma

### Visual Studio'da:
1. **Stop (Shift + F5)** - Backend'i durdur
2. **Start (F5)** - Yeniden baslat
3. Tarayicida Swagger acilacak: `https://localhost:7297/swagger`

### Komut Satirindan:
```bash
# Projeyi durdur (Ctrl + C)
# Yeniden baslat
cd SOSYAL_KUTUPHANE_PLATFORMU
dotnet run
```

---

## ?? CORS Testi

### 1. Browser Console'da Test:
Frontend'inizde (http://localhost:3001) browser console'u acin (F12):

```javascript
// Test 1: Basit GET istegi
fetch('https://localhost:7297/api/auth/me', {
  method: 'GET',
  headers: {
    'Authorization': 'Bearer YOUR_TOKEN_HERE'
  }
})
.then(response => response.json())
.then(data => console.log('Basarili:', data))
.catch(error => console.error('Hata:', error));

// Test 2: POST istegi (Login)
fetch('https://localhost:7297/api/auth/giris', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'test@test.com',
    password: '123456'
  })
})
.then(response => response.json())
.then(data => console.log('Login basarili:', data))
.catch(error => console.error('Hata:', error));
```

### 2. Network Tab'de Kontrol:
1. Browser'da **F12** > **Network** sekmesi
2. Bir istek atin (ornek: login)
3. Istegin **Headers** bolumunu kontrol edin:

**Basarili CORS Response Headers:**
```
Access-Control-Allow-Origin: http://localhost:3001
Access-Control-Allow-Credentials: true
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: *
```

---

## ? CORS Hatalari ve Cozumleri

### Hata 1: "CORS policy blocked"
```
Access to fetch at 'https://localhost:7297/api/auth/giris' from origin 
'http://localhost:3001' has been blocked by CORS policy
```

**Cozum:**
? Zaten cozuldu! `http://localhost:3001` CORS policy'ye eklendi.

---

### Hata 2: "No 'Access-Control-Allow-Origin' header"
```
Response to preflight request doesn't pass access control check: 
No 'Access-Control-Allow-Origin' header is present
```

**Cozum:**
1. Backend'in calistigini kontrol edin
2. `Program.cs`'de `app.UseCors("AllowFrontend")` satirinin oldugunu kontrol edin
3. Backend'i yeniden baslatin

---

### Hata 3: "Credentials flag is true, but Access-Control-Allow-Credentials is not"
```
Credentials mode is 'include', but the Access-Control-Allow-Credentials 
header is ''
```

**Cozum:**
? Zaten var: `.AllowCredentials()` CORS policy'de ekli.

---

### Hata 4: "Preflight request failed (OPTIONS)"
```
Failed to load resource: the server responded with a status of 405 (Method Not Allowed)
```

**Cozum:**
Backend otomatik olarak OPTIONS isteklerini handle ediyor. 
Eger sorun devam ederse:
```csharp
app.MapMethods("/api/{**catch-all}", new[] { "OPTIONS" }, 
    () => Results.Ok());
```

---

## ?? Frontend Axios Konfigurasyonu

### axios.defaults.baseURL Ayari:
```javascript
// src/api/axiosConfig.js
import axios from 'axios';

// Backend base URL
axios.defaults.baseURL = 'https://localhost:7297/api';

// Credentials (cookies icin)
axios.defaults.withCredentials = true;

// Her istege token ekle
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Hata yakalama
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token suresi dolmus, login'e yonlendir
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axios;
```

---

## ?? HTTPS Sertifika Sorunu (Development)

### Hata: "NET::ERR_CERT_AUTHORITY_INVALID"
Development'da self-signed certificate kullanilir ve browser guvenmedigi icin hata verebilir.

### Cozum 1: Tarayicida Kabul Et:
1. `https://localhost:7297` adresine git
2. "Advanced" > "Proceed to localhost (unsafe)" tikla

### Cozum 2: Development Certificate'i Guvenilir Yap:
```bash
# Windows'da
dotnet dev-certs https --trust

# Bu komut Windows sertifika deposuna ekler
```

### Cozum 3: HTTP Kullan (Guvenli Degil):
```csharp
// Program.cs (sadece development icin)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // Bu satiri yoruma al
}
```
Sonra frontend'de:
```javascript
axios.defaults.baseURL = 'http://localhost:5000/api'; // HTTPS yerine HTTP
```

---

## ?? Kontrol Listesi

Backend'i calistirmadan once:

- [ ] `Program.cs`'de CORS policy var mi?
- [ ] Frontend port numarasi (`http://localhost:3001`) CORS'da var mi?
- [ ] `app.UseCors("AllowFrontend")` satiri `app.UseAuthentication()`'dan once mi?
- [ ] Backend calisiyor mu? (`dotnet run` veya Visual Studio F5)
- [ ] Swagger aciliyor mu? (`https://localhost:7297/swagger`)
- [ ] Frontend axios'ta `baseURL` dogru mu?
- [ ] Frontend axios'ta `withCredentials: true` var mi?

---

## ?? Son Test

### Backend:
```bash
cd SOSYAL_KUTUPHANE_PLATFORMU
dotnet run
```

Cikti:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7297
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Frontend (React):
```bash
npm start
# veya
yarn start
```

### Browser'da Test:
```
Frontend: http://localhost:3001
Backend Swagger: https://localhost:7297/swagger
```

Login sayfasinda email/sifre gir ve Network tab'de:
- ? Status: 200 OK
- ? Response: `{ "success": true, "data": { "token": "..." } }`
- ? Headers: `Access-Control-Allow-Origin: http://localhost:3001`

---

## ?? Debug Modu

Eger hala CORS sorunu varsa, backend'de loglari etkinlestir:

### Program.cs'e ekle:
```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
```

### CORS detayli log:
```csharp
app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:3001")
   .AllowAnyMethod()
  .AllowAnyHeader()
 .AllowCredentials();
 
    // Debug icin
    Console.WriteLine("CORS Policy applied for: http://localhost:3001");
});
```

---

## ?? Yardim

### CORS Hala Calismiyor?

1. **Browser cache'i temizle:** Ctrl + Shift + Delete
2. **Incognito/Private mode'da dene**
3. **Backend'i tam olarak yeniden baslat:** Visual Studio'yu kapat ve ac
4. **Port cakismasini kontrol et:**
   ```bash
   netstat -ano | findstr :7297
   # Port kullaniliyor mu?
   ```
5. **Firewall/Antivirus'u gecici devre disi birak**

---

Son Guncelleme: 2024
Durum: ? CORS AYARI TAMAMLANDI - FRONTEND BAGLANTI HAZIR
