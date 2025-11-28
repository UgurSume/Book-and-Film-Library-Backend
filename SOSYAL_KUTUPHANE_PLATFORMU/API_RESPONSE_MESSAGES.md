# API Response Mesajlari - Turkce (Karakter Kullanilmadan)

## Not
Bu dokuman, API'nin dondurdugu tum kullanici mesajlarini icerir.
Turkce karakterler (ý, þ, ü, ö, ç, ð, Ý, Þ, Ü, Ö, Ç, Ð) kullanilmamistir.

---

## Authentication (AuthController)

### Basarili Mesajlar:
- `"Kayit basarili."` - Kayit islemi tamamlandi
- `"Giris basarili."` - Giris yapildi
- `"Sifreniz basariyla guncellendi. Artik giris yapabilirsiniz."` - Sifre sifirlama basarili
- `"Token gecerli."` - Reset token dogrulandi
- `"Eger bu email kayitliysa, sifre sifirlama linki gonderildi."` - Sifre sifirlama talebi

### Hata Mesajlari:
- `"Bu kullanici adi veya email zaten kullaniliyor."` - Duplicate kullanici
- `"Email veya sifre hatali."` - Giris hatasi
- `"Gecersiz veya suresi dolmus token."` - Reset token gecersiz
- `"Bu token zaten kullanilmis."` - Token kullanilmis
- `"Token'in suresi dolmus. Lutfen yeni bir sifre sifirlama talebi olusturun."` - Token suresi dolmus
- `"Token gecersiz veya suresi dolmus."` - Token dogrulama hatasi

---

## Activity (ActivityController)

### Basarili Mesajlar:
- `"Aktivite begenildi."` - Like basarili
- `"Begeni geri alindi."` - Unlike basarili
- `"Yorum basariyla eklendi."` - Comment eklendi
- `"Yorumlar basariyla getirildi."` - Comments listelendi
- `"Yorum basariyla guncellendi."` - Comment update
- `"Yorum basariyla silindi."` - Comment delete
- `"Begeniler basariyla getirildi."` - Likes listelendi

### Hata Mesajlari:
- `"Aktivite bulunamadi."` - Activity not found
- `"Bu aktiviteyi zaten begendiniz."` - Already liked
- `"Bu aktiviteyi begenmemissiniz."` - Not liked yet
- `"Yorum bulunamadi."` - Comment not found

---

## Content (ContentController)

### Basarili Mesajlar:
- `"Icerik zaten mevcut."` - Content already exists
- `"Icerik basariyla eklendi."` - Content added
- `"Puan basariyla kaydedildi."` - New rating
- `"Puan guncellendi."` - Rating updated
- `"Yorum basariyla kaydedildi."` - Review added
- `"Icerik detaylari basariyla getirildi."` - Content details
- `"Yorum basariyla guncellendi."` - Review updated
- `"Yorum basariyla silindi."` - Review deleted
- `"Puaniniz basariyla getirildi."` - User rating fetched
- `"Yorumunuz basariyla getirildi."` - User review fetched

### Hata Mesajlari:
- `"Icerik bulunamadi."` - Content not found
- `"Bu icerik hakkinda zaten yorum yaptiniz. Yorumunuzu duzenleyebilirsiniz."` - Duplicate review
- `"Yorum bulunamadi."` - Review not found
- `"Bu iceriðe henuz puan vermediniz."` - No rating yet
- `"Bu icerik hakkinda henuz yorum yapmadiniz."` - No review yet

---

## User (UserController)

### Basarili Mesajlar:
- `"Profil basariyla getirildi."` - Profile fetched
- `"Profil basariyla guncellendi."` - Profile updated
- `"Kullanici basariyla takip edildi."` - Follow successful
- `"Kullanici takipten cikarildi."` - Unfollow successful
- `"Takipciler basariyla getirildi."` - Followers listed
- `"Takip edilenler basariyla getirildi."` - Following listed

### Hata Mesajlari:
- `"Kullanici bulunamadi."` - User not found
- `"Kendinizi takip edemezsiniz."` - Cannot follow self
- `"Bu kullaniciyi zaten takip ediyorsunuz."` - Already following
- `"Kendinizi takipten cikaramazsiniz."` - Cannot unfollow self
- `"Bu kullaniciyi takip etmiyorsunuz."` - Not following

---

## Library (LibraryController)

### Basarili Mesajlar:
- `"Listeler basariyla getirildi."` - Lists fetched
- `"Listeleriniz basariyla getirildi."` - My lists fetched
- `"Icerik basariyla listeye eklendi."` - Added to list
- `"Icerik listeden kaldirildi."` - Removed from list

### Hata Mesajlari:
- `"Kullanici bulunamadi."` - User not found
- `"Liste bulunamadi veya bu kullaniciya ait degil."` - List not found or not owned
- `"Icerik bulunamadi."` - Content not found
- `"Bu icerik zaten listede mevcut."` - Already in list
- `"Bu icerik listede bulunamadi."` - Not in list

---

## Search (SearchController)

### Basarili Mesajlar:
- `"'{query}' icin {count} film bulundu."` - Movies search result
- `"'{query}' icin {count} kitap bulundu."` - Books search result
- `"'{query}' icin toplam {count} sonuc bulundu."` - All search result

### Hata Mesajlari:
- `"Arama sorgusu bos olamaz."` - Empty search query

---

## Feed (FeedController)

### Basarili Mesajlar:
- `"Feed basariyla getirildi."` - Main feed
- `"Kullanici aktiviteleri basariyla getirildi."` - User feed
- `"Kesfet feed'i basariyla getirildi."` - Explore feed
- `"Henuz kimseyi takip etmiyorsunuz."` - Empty feed message

### Hata Mesajlari:
- `"Kullanici bulunamadi."` - User not found

---

## Discover (DiscoverController)

### Basarili Mesajlar:
- `"En yuksek puanli icerikler basariyla getirildi."` - Top rated
- `"Populer icerikler basariyla getirildi."` - Popular
- `"Son {days} gunun trend icerikleri basariyla getirildi."` - Trending
- `"Filtrelenmis icerikler basariyla getirildi."` - Filtered
- `"Yeni eklenen icerikler basariyla getirildi."` - Recent
- `"Onerilen icerikler basariyla getirildi."` - Recommended

### Hata Mesajlari:
- `"Gecersiz filtre parametreleri"` - Invalid filter

---

## Validation Hatalari (DTO'lar)

### RegisterRequest:
- `"Kullanici adi zorunludur"`
- `"Kullanici adi 3-50 karakter arasinda olmalidir"`
- `"E-posta zorunludur"`
- `"Gecerli bir e-posta adresi giriniz"`
- `"Sifre zorunludur"`
- `"Sifre en az 6 karakter olmalidir"`
- `"Sifre tekrari zorunludur"`
- `"Sifreler eslesmiyor"`

### LoginRequest:
- `"E-posta zorunludur"`
- `"Gecerli bir e-posta adresi giriniz"`
- `"Sifre zorunludur"`

### RateContentRequest:
- `"Icerik ID zorunludur"`
- `"Puan zorunludur"`
- `"Puan 1-10 arasinda olmalidir"`

### ReviewContentRequest:
- `"Icerik ID zorunludur"`
- `"Yorum metni zorunludur"`
- `"Yorum 10-2000 karakter arasinda olmalidir"`

### CreateListRequest:
- `"Liste adi zorunludur"`
- `"Liste adi 1-100 karakter arasinda olmalidir"`
- `"Aciklama maksimum 500 karakter olabilir"`

### UpdateProfileRequest:
- `"Kullanici adi 3-50 karakter arasinda olmalidir"`
- `"Biyografi maksimum 500 karakter olabilir"`
- `"Gecerli bir URL giriniz"`

### ContentFilterRequest:
- `"Icerik turu 'movie' veya 'book' olmalidir"`
- `"Minimum puan 1-10 arasinda olmalidir"`
- `"Maksimum puan 1-10 arasinda olmalidir"`
- `"Gecerli bir yil giriniz"`
- `"Gecerli siralama: rating_desc, rating_asc, popular, recent"`
- `"Skip 0 veya daha buyuk olmalidir"`
- `"Take 1-100 arasinda olmalidir"`

### AddActivityCommentRequest:
- `"Yorum metni gereklidir."`
- `"Yorum en az 1 karakter olmalidir."`
- `"Yorum en fazla 500 karakter olabilir."`

---

## Liste Sistemine Ozel Isimlendirmeler

### Varsayilan Listeler (Default Lists):
Film icin:
- `"Izlediklerim"` (Watched)
- `"Izlenecekler"` (To Watch)

Kitap icin:
- `"Okuduklarim"` (Read)
- `"Okunacaklar"` (To Read)

---

## Swagger Endpoint Aciklamalari

### ActivityController:
- `"Aktiviteyi begen"` - POST /api/activity/{activityId}/like
- `"Aktivite begenisini geri al"` - DELETE /api/activity/{activityId}/unlike
- `"Aktiviteye yorum yap"` - POST /api/activity/{activityId}/comment
- `"Aktivitedeki yorumlari getir"` - GET /api/activity/{activityId}/comments
- `"Yorumu guncelle (sadece kendi yorumu)"` - PUT /api/activity/comment/{commentId}
- `"Yorumu sil (sadece kendi yorumu)"` - DELETE /api/activity/comment/{commentId}
- `"Aktiviteyi begenen kullanicilari getir"` - GET /api/activity/{activityId}/likes

### ContentController:
- `"Iceriði veritabaninda garantiye al"` - POST /api/content/ensure
- `"Puan verme (varsa guncelle, yoksa ekle)"` - POST /api/content/rate
- `"Yorum ekleme"` - POST /api/content/review
- `"Icerik detay + ortalama puan + yorumlar + kullanici durumu"` - GET /api/content/{id}
- `"Yorum duzenleme (sadece kendi yorumunu duzenleyebilir)"` - PUT /api/content/review/{id}
- `"Yorum silme (sadece kendi yorumunu silebilir)"` - DELETE /api/content/review/{id}
- `"Kullanicinin bir iceriðe verdigi puani getir"` - GET /api/content/{contentId}/my-rating
- `"Kullanicinin bir icerik hakkindaki yorumunu getir"` - GET /api/content/{contentId}/my-review

---

## Genel Bilgiler

### API Response Format:
```json
{
  "success": true/false,
  "message": "Mesaj metni",
  "data": { ... },
  "errors": ["Hata 1", "Hata 2"]
}
```

### Paged Response Format:
```json
{
  "success": true,
  "message": "Mesaj",
  "data": {
    "items": [...],
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalCount": 95,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

---

## Onemli Notlar

1. **Turkce Karakterler**: Hiçbir API response'unda Turkce karakter (ý, þ, ü, ö, ç, ð) kullanilmamistir.
2. **Tutarlilik**: Tum mesajlar kucuk harfle baslar (isim haric).
3. **Teknik Terimler**: API endpoint isimleri, property isimleri ve enum degerleri Ingilizce'dir.
4. **Kullanici Mesajlari**: Sadece kullaniciya gosterilecek mesajlar Turkce (karaktersiz) yazilmistir.
5. **Swagger UI**: Tum endpoint aciklamalari da Turkce karaktersiz yapilmistir.

---

Son Guncelleme: 2024
Durum: Tum controller'lar guncellendi
