# API Endpoint URL'leri - Turkce (Karakter Kullanilmadan)

## Not
Bu dokuman, API endpoint URL'lerinin tam listesini icerir.
Turkce karakterler (ý, þ, ü, ö, ç, ð, Ý, Þ, Ü, Ö, Ç, Ð) KULLANILMAMIÞTIR.

---

## Activity Endpoints

### Begeni Islemleri:
- `POST /api/activity/{activityId}/begen` - Aktiviteyi begen
- `DELETE /api/activity/{activityId}/begeniyi-geri-al` - Begeniyi geri al
- `GET /api/activity/{activityId}/begeniler` - Begenen kullanicilari listele

### Yorum Islemleri:
- `POST /api/activity/{activityId}/yorum` - Aktiviteye yorum yap
- `GET /api/activity/{activityId}/yorumlar` - Yorumlari listele
- `PUT /api/activity/yorum/{commentId}` - Yorumu guncelle
- `DELETE /api/activity/yorum/{commentId}` - Yorumu sil

---

## Auth Endpoints

### Kimlik Dogrulama:
- `POST /api/auth/kayit` - Yeni kullanici kaydý
- `POST /api/auth/giris` - Kullanici girisi
- `POST /api/auth/sifremi-unuttum` - Sifre sifirlama talebi
- `POST /api/auth/sifre-sifirla` - Sifre sifirla
- `GET /api/auth/token-dogrula/{token}` - Token dogrulama

---

## Content Endpoints

### Icerik Islemleri:
- `POST /api/content/ensure` - Iceriði ekle/kontrol et
- `POST /api/content/rate` - Puan ver
- `GET /api/content/{id}` - Icerik detaylari
- `GET /api/content/{contentId}/my-rating` - Benim puanim
- `GET /api/content/{contentId}/my-review` - Benim yorumum

### Yorum Islemleri:
- `POST /api/content/review` - Yorum ekle
- `PUT /api/content/review/{id}` - Yorum guncelle
- `DELETE /api/content/review/{id}` - Yorum sil

---

## User Endpoints

### Profil Islemleri:
- `GET /api/user/profile/{userId}` - Kullanici profili
- `GET /api/user/my-profile` - Benim profilim
- `PUT /api/user/update-profile` - Profil guncelle

### Takip Islemleri:
- `POST /api/user/follow/{userId}` - Takip et
- `DELETE /api/user/unfollow/{userId}` - Takibi birak
- `GET /api/user/{userId}/followers` - Takipciler
- `GET /api/user/{userId}/following` - Takip edilenler
- `GET /api/user/my-followers` - Benim takipcilerim
- `GET /api/user/my-following` - Benim takip ettiklerim

---

## Library Endpoints

### Liste Islemleri:
- `GET /api/library/lists/{userId}` - Kullanici listeleri
- `GET /api/library/my-lists` - Benim listelerim
- `POST /api/library/add` - Listeye ekle
- `DELETE /api/library/remove` - Listeden cikar
- `POST /api/library/create-list` - Liste olustur
- `PUT /api/library/update-list/{listId}` - Liste guncelle
- `DELETE /api/library/delete-list/{listId}` - Liste sil

---

## Feed Endpoints

### Akis Islemleri:
- `GET /api/feed` - Ana akis (takip edilenler)
- `GET /api/feed/user/{userId}` - Kullanici akisi
- `GET /api/feed/my` - Benim akisim
- `GET /api/feed/explore` - Kesfet akisi

---

## Discover Endpoints

### Kesif Islemleri:
- `GET /api/discover/top-rated` - En yuksek puanlilar
- `GET /api/discover/popular` - Populer icerikler
- `GET /api/discover/trending` - Trend olanlar
- `POST /api/discover/filter` - Filtrele
- `GET /api/discover/recent` - Yeni eklenenler
- `GET /api/discover/recommended` - Onerilenler

---

## Search Endpoints

### Arama Islemleri:
- `GET /api/search/movies` - Film ara
- `GET /api/search/books` - Kitap ara
- `GET /api/search/all` - Tum icerikler (film + kitap)

---

## Ornek Kullanim

### JavaScript/Axios:
```javascript
// Aktiviteyi begen
axios.post('/api/activity/123/begen', {}, {
  headers: { Authorization: `Bearer ${token}` }
});

// Kullanici girisi
axios.post('/api/auth/giris', {
  email: 'user@example.com',
  password: '123456'
});

// Icerik ara
axios.get('/api/search/movies?query=inception&pageNumber=1&pageSize=20');

// Profil guncelle
axios.put('/api/user/update-profile', {
  userName: 'yenikullanici',
  biography: 'Benim biyografim'
}, {
  headers: { Authorization: `Bearer ${token}` }
});
```

### C# / HttpClient:
```csharp
// Aktiviteyi begen
var response = await httpClient.PostAsync("/api/activity/123/begen", null);

// Kullanici girisi
var loginData = new { email = "user@example.com", password = "123456" };
var response = await httpClient.PostAsJsonAsync("/api/auth/giris", loginData);

// Icerik ara
var response = await httpClient.GetAsync("/api/search/movies?query=inception");
```

---

## Onemli Notlar

### URL Yapisi:
1. **Turkce Karaktersiz**: Tum URL'ler Turkce ama karakter kullanilmadan
2. **Kebab-case**: Cok kelimeli endpoint'ler tire(-) ile ayrilir
   - `sifremi-unuttum`
   - `begeniyi-geri-al`
   - `token-dogrula`

### Yonlendirme:
- Eski endpoint'ler: `/api/auth/forgot-password`
- Yeni endpoint'ler: `/api/auth/sifremi-unuttum`

### Geriye Uyumluluk:
Frontend guncellenene kadar eski endpoint'ler de calismaya devam edebilir.
Ancak tum yeni gelistirmeler yeni endpoint'leri kullanmali.

---

## Swagger UI'da Gorunum

Swagger'da su sekilde gorunecek:

**Activity**
- POST /api/activity/{activityId}/begen
- DELETE /api/activity/{activityId}/begeniyi-geri-al
- POST /api/activity/{activityId}/yorum
- GET /api/activity/{activityId}/yorumlar

**Auth**
- POST /api/auth/kayit
- POST /api/auth/giris
- POST /api/auth/sifremi-unuttum
- POST /api/auth/sifre-sifirla

---

Son Guncelleme: 2024
Durum: Tum endpoint URL'leri Turkce karaktersiz
