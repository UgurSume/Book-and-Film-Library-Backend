# Frontend Entegrasyonu - Auth Endpoint'leri

## ? Mevcut Endpoint'ler

### 1. Kullanici Kayit (Register)
```
POST https://localhost:7297/api/auth/kayit
```

**Request Body:**
```json
{
  "userName": "testkullanici",
  "email": "test@example.com",
  "password": "123456",
  "confirmPassword": "123456"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Kayit basarili.",
  "data": {
    "userId": 1,
    "userName": "testkullanici",
    "email": "test@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Error Response (400):**
```json
{
  "success": false,
  "message": "Bu kullanici adi veya email zaten kullaniliyor.",
  "errors": null
}
```

**Validation Errors:**
```json
{
  "success": false,
  "message": "Gecersiz veri",
  "errors": [
    "Kullanici adi zorunludur",
    "E-posta zorunludur",
    "Sifre en az 6 karakter olmalidir",
    "Sifreler eslesmiyor"
  ]
}
```

---

### 2. Kullanici Girisi (Login)
```
POST https://localhost:7297/api/auth/giris
```

**Request Body:**
```json
{
  "email": "test@example.com",
  "password": "123456"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Giris basarili.",
  "data": {
    "userId": 1,
    "userName": "testkullanici",
    "email": "test@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Error Response (401):**
```json
{
  "success": false,
  "message": "Email veya sifre hatali.",
  "errors": null
}
```

---

### 3. Giris Yapmis Kullanici Bilgileri (Me) ? YENÝ!
```
GET https://localhost:7297/api/auth/me
Authorization: Bearer {token}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Kullanici bilgileri basariyla getirildi.",
  "data": {
 "userId": 1,
    "userName": "testkullanici",
    "email": "test@example.com",
    "avatarUrl": null,
    "biography": null,
    "followersCount": 0,
    "followingCount": 0,
    "createdAt": "2024-01-01T10:00:00Z"
  }
}
```

**Error Response (401):**
```json
{
  "success": false,
  "message": "Gecersiz token.",
  "errors": null
}
```

---

## ?? Token Kullanimi

### Token Alma:
1. `/api/auth/kayit` veya `/api/auth/giris` endpoint'ine istek atin
2. Response'dan `data.token` alanini alin
3. Bu token'i localStorage veya sessionStorage'a kaydedin

### Token Gonderme:
Tum korunmus endpoint'lere Authorization header'i ile gonderin:

```javascript
// JavaScript/Axios Ornegi
const token = localStorage.getItem('authToken');

axios.get('https://localhost:7297/api/auth/me', {
  headers: {
 'Authorization': `Bearer ${token}`
  }
});
```

---

## ??? Veritabani (SQL Server - SSMS)

### Connection String:
`appsettings.json` dosyasinda:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SosyalKutuphanePlatformu;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Veritabani Olusturma:
```bash
# Migration uygula (ilk kez calistiriyorsaniz)
dotnet ef database update
```

### SSMS'de Kontrol:
1. SQL Server Management Studio'yu acin
2. `localhost` sunucusuna baglanin
3. `SosyalKutuphanePlatformu` veritabanini goreceksiniz
4. Tablolar:
   - `Users` - Kullanici bilgileri
   - `Contents` - Film/Kitap verileri
   - `Ratings` - Kullanici puanlari
   - `Reviews` - Kullanici yorumlari
   - `UserLists` - Kullanici listeleri
   - `Activities` - Kullanici aktiviteleri
   - `UserFollowers` - Takip iliskileri

---

## ?? Test Etme

### Swagger UI ile Test:
1. Projeyi calistirin: `dotnet run`
2. Tarayicida acin: `https://localhost:7297/swagger`
3. Auth endpoint'lerini test edin

### Postman ile Test:

#### 1. Kayit Testi:
```
POST https://localhost:7297/api/auth/kayit
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@test.com",
  "password": "123456",
  "confirmPassword": "123456"
}
```

#### 2. Giris Testi:
```
POST https://localhost:7297/api/auth/giris
Content-Type: application/json

{
  "email": "test@test.com",
  "password": "123456"
}
```

#### 3. Me Endpoint Testi:
```
GET https://localhost:7297/api/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Frontend Entegrasyonu (React Ornegi)

### 1. Login Componenti:
```javascript
import axios from 'axios';

const API_URL = 'https://localhost:7297/api';

const login = async (email, password) => {
  try {
    const response = await axios.post(`${API_URL}/auth/giris`, {
      email,
      password
    });

    if (response.data.success) {
// Token'i kaydet
      localStorage.setItem('authToken', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data));
      
      return response.data;
    }
  } catch (error) {
    console.error('Giris hatasi:', error.response?.data?.message);
    throw error;
  }
};
```

### 2. Register Componenti:
```javascript
const register = async (userName, email, password, confirmPassword) => {
  try {
    const response = await axios.post(`${API_URL}/auth/kayit`, {
  userName,
      email,
   password,
      confirmPassword
    });

    if (response.data.success) {
      // Kayit basarili, token'i kaydet
      localStorage.setItem('authToken', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data));
      
  return response.data;
    }
  } catch (error) {
    console.error('Kayit hatasi:', error.response?.data);
    throw error;
  }
};
```

### 3. Axios Interceptor (Token Otomatik Gonderme):
```javascript
// axiosConfig.js
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'https://localhost:7297/api'
});

// Her istege token ekle
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export default axiosInstance;
```

### 4. Protected Route Ornegi:
```javascript
// ProtectedRoute.jsx
import { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import axiosInstance from './axiosConfig';

const ProtectedRoute = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(null);

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await axiosInstance.get('/auth/me');
        setIsAuthenticated(response.data.success);
      } catch (error) {
        setIsAuthenticated(false);
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
 }
    };

    checkAuth();
  }, []);

  if (isAuthenticated === null) {
    return <div>Yukleniyor...</div>;
  }

  return isAuthenticated ? children : <Navigate to="/login" />;
};

export default ProtectedRoute;
```

---

## ?? Guvenlik Notlari

1. **HTTPS Kullanin:** Production'da mutlaka HTTPS
2. **Token Guvenligini Saglyin:** XSS saldirilarindan korunun
3. **CORS Ayarlarini Yapin:** `Program.cs`'de zaten var
4. **Password Hashing:** BCrypt kullaniliyor ?
5. **JWT Secret:** User Secrets'da saklanmali ?

---

## ? Sik Karsilasilan Hatalar

### 1. CORS Hatasi:
```
Access to XMLHttpRequest at 'https://localhost:7297/api/auth/giris' 
from origin 'http://localhost:3000' has been blocked by CORS policy
```
**Cozum:** `Program.cs`'de CORS zaten ayarli, frontend URL'inizi ekleyin:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
  policy.WithOrigins("http://localhost:3000") // Frontend URL
              .AllowAnyHeader()
     .AllowAnyMethod()
              .AllowCredentials();
});
});
```

### 2. 401 Unauthorized:
- Token'i dogru gonderdiginizden emin olun
- Token'in suresi dolmus olabilir (24 saat)
- Token formatini kontrol edin: `Bearer {token}`

### 3. Veritabani Baglanti Hatasi:
- SQL Server calistigini kontrol edin
- Connection string'i dogru yazildi mi?
- Veritabani olusturuldu mu? (`dotnet ef database update`)

---

## ?? Diger Endpoint'ler

### Profil Guncelleme:
```
PUT https://localhost:7297/api/user/update-profile
Authorization: Bearer {token}

{
  "userName": "yenikullaniciadi",
  "biography": "Benim biyografim",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

### Sifre Sifirlama:
```
1. POST /api/auth/sifremi-unuttum
   Body: { "email": "test@test.com" }

2. Emaildeki linke tikla

3. POST /api/auth/sifre-sifirla
   Body: { 
     "token": "...", 
     "email": "test@test.com",
     "newPassword": "yenisifre123",
     "confirmPassword": "yenisifre123"
   }
```

---

Son Guncelleme: 2024
Durum: ? FRONTEND ENTEGRASYONU ICIN HAZIR
