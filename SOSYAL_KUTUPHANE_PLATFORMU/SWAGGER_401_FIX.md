# ?? SWAGGER'DA 401 HATASI ÇÖZÜMÜ

## ? **Sorun**
```
401 Unauthorized
/api/content/ensure endpoint'i çaðrýlamýyor
```

## ? **Neden Oluyor?**
`ContentController`'daki bazý endpoint'ler **`[Authorize]`** attribute'u ile korunmuþ:
- `/api/Content/ensure` ? ? Token Gerekli
- `/api/Content/rate` ? ? Token Gerekli
- `/api/Content/review` ? ? Token Gerekli
- `/api/Content/{id}/my-rating` ? ? Token Gerekli
- `/api/Content/{id}/my-review` ? ? Token Gerekli

## ?? **ÇÖZÜM: Swagger'da Token Kullanma**

### **Adým 1: Kayýt Ol veya Giriþ Yap**

#### **1.1 Swagger'ý Aç:**
```
https://localhost:7297/swagger
```

#### **1.2 Register Endpoint'ini Bul:**
```
POST /api/Auth/register
```

**Request Body:**
```json
{
  "userName": "testuser",
  "email": "test@test.com",
"password": "Test123!",
  "confirmPassword": "Test123!"
}
```

**Execute** butonuna týkla.

**Response:**
```json
{
  "success": true,
  "message": "Kayit basarili. Giris yapabilirsiniz.",
  "data": null,
  "errors": null
}
```

---

#### **1.3 Login Endpoint'ini Bul:**
```
POST /api/Auth/login
```

**Request Body:**
```json
{
  "email": "test@test.com",
  "password": "Test123!"
}
```

**Execute** butonuna týkla.

**Response:**
```json
{
  "success": true,
  "message": "Giris basarili.",
"data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYmYiOjE3MDEwMDAwMDAsImV4cCI6MTcwMTA4NjQwMCwiaWF0IjoxNzAxMDAwMDAwfQ.xyz123...",
    "userId": 1,
    "userName": "testuser",
    "email": "test@test.com"
  },
  "errors": null
}
```

---

### **Adým 2: Token'ý Swagger'a Ekle**

#### **2.1 Token'ý Kopyala:**
Response'daki `token` deðerini kopyala (tüm string'i):
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYmYiOjE3MDEwMDAwMDAsImV4cCI6MTcwMTA4NjQwMCwiaWF0IjoxNzAxMDAwMDAwfQ.xyz123...
```

#### **2.2 Swagger'da Authorize Butonuna Týkla:**
Swagger sayfasýnýn sað üst köþesinde **yeþil "Authorize" ??** butonu var.

#### **2.3 Token'ý Yapýþtýr:**
Modal açýlýnca:
```
Value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.xyz123...
```

**ÖNEMLÝ:** Token'ýn baþýna `Bearer ` (boþluklu) ekle!

#### **2.4 Authorize Butonuna Týkla:**
Modal kapanýr ve artýk token tüm isteklere otomatik eklenir.

---

### **Adým 3: Ensure Content'i Test Et**

#### **3.1 Endpoint'i Bul:**
```
POST /api/Content/ensure
```

#### **3.2 Request Body:**
```json
{
  "externalId": "550",
  "type": "movie",
  "title": "Fight Club",
  "posterPath": "/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",
  "releaseDate": "1999-10-15",
  "overview": "An insomniac office worker and a devil-may-care soapmaker form an underground fight club.",
  "director": "David Fincher",
  "cast": ["Brad Pitt", "Edward Norton", "Helena Bonham Carter"],
  "genres": ["Drama", "Thriller"]
}
```

#### **3.3 Execute:**
**Artýk 200 OK almalýsýnýz!**

**Response:**
```json
{
  "success": true,
  "message": "Icerik basariyla eklendi.",
  "data": {
    "contentId": 1
  },
  "errors": null
}
```

---

## ?? **ALTERNATIF: Token Gereksinimleri Kaldýrma (Test Ýçin)**

Eðer sadece test için token zorluðunu kaldýrmak istiyorsanýz:

### **Geçici Çözüm (Sadece Development Ýçin):**

```csharp
// ContentController.cs

[HttpPost("ensure")]
[AllowAnonymous]  // Token olmadan da çalýþýr (sadece test için!)
public async Task<ActionResult<ApiResponse<object>>> EnsureContent([FromBody] EnsureContentRequest model)
{
    // Kullanýcý ID'si gerekiyorsa:
    int userId = User.Identity?.IsAuthenticated == true 
        ? GetCurrentUserId() 
 : 1;  // Test için varsayýlan kullanýcý
    
    // ... geri kalan kod
}
```

**?? UYARI:** Bu sadece development/test için! Production'da asla `[AllowAnonymous]` kullanmayýn!

---

## ?? **Swagger Test Checklist**

### Token Olmadan Çalýþan Endpoint'ler:
- [x] `POST /api/Auth/register` - Kayýt
- [x] `POST /api/Auth/login` - Giriþ
- [x] `GET /api/Content/{id}` - Content detaylarý
- [x] `GET /api/Content/{id}/reviews` - Yorumlar
- [x] `GET /api/Search/movies` - Film ara
- [x] `GET /api/Search/books` - Kitap ara

### Token Gerektiren Endpoint'ler:
- [ ] `POST /api/Content/ensure` - Content ekle
- [ ] `POST /api/Content/rate` - Puan ver
- [ ] `POST /api/Content/review` - Yorum ekle
- [ ] `GET /api/Content/{id}/my-rating` - Kullanýcýnýn puaný
- [ ] `GET /api/Content/{id}/my-review` - Kullanýcýnýn yorumu
- [ ] `POST /api/Library/add` - Listeye ekle
- [ ] `GET /api/Library/my-lists` - Listelerim
- [ ] `POST /api/User/follow` - Takip et
- [ ] `GET /api/Feed` - Ana akýþ

---

## ?? **Hýzlý Test Scripti**

```javascript
// Browser Console'da çalýþtýr

// 1. Register
const registerResponse = await fetch('https://localhost:7297/api/Auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userName: 'testuser',
    email: 'test@test.com',
    password: 'Test123!',
    confirmPassword: 'Test123!'
  })
});
console.log('Register:', await registerResponse.json());

// 2. Login
const loginResponse = await fetch('https://localhost:7297/api/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'test@test.com',
    password: 'Test123!'
  })
});
const loginData = await loginResponse.json();
const token = loginData.data.token;
console.log('Token:', token);

// 3. Ensure Content (Token ile)
const ensureResponse = await fetch('https://localhost:7297/api/Content/ensure', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    externalId: '550',
    type: 'movie',
    title: 'Fight Club',
    posterPath: '/test.jpg',
releaseDate: '1999-10-15',
    overview: 'Test movie'
  })
});
console.log('Ensure Content:', await ensureResponse.json());
```

---

## ?? **Özet**

### Problem:
```
401 Unauthorized - Token eksik
```

### Çözüm:
```
1. Swagger'da /api/Auth/register ile kayýt ol
2. /api/Auth/login ile giriþ yap
3. Token'ý kopyala
4. Swagger'da "Authorize" butonuna týkla
5. Token'ý yapýþtýr (Bearer ile birlikte)
6. Artýk tüm endpoint'leri test edebilirsin!
```

---

**Oluþturulma Tarihi:** 2024-12-04  
**Durum:** ? ÇÖZÜM HAZIR
