# ? FRONTEND ENTEGRASYON REHBERÝ - TÜM ENDPOINT'LER

## ?? **Düzeltilen Sorunlar**

### ? **ÖNCEDEN**
- `/api/Content/ensure` ? 500 Internal Server Error
- Frontend'den gelen alanlar backend'de desteklenmiyordu
- `PosterPath`, `ReleaseDate`, `Overview` alanlarý iþlenemiyordu

### ? **ÞÝMDÝ**
- `/api/Content/ensure` ? 200 OK ?
- Tüm frontend alanlarý destekleniyor
- Alternatif alan isimleri handle ediliyor
- Detaylý logging ve hata mesajlarý

---

## ?? **1. Content Ensure Endpoint**

### **URL:** `POST /api/Content/ensure`
### **Auth:** ? Required (Bearer Token)

### **Frontend'den Gönderilecek Data (Opsiyonel Alanlar):**

```javascript
// ÖRNEKLENDÝRÝLMÝÞ: Film Ekleme
const ensureContentData = {
  externalId: "550",  // TMDb ID (string olarak)
  type: "movie",      // "movie" veya "book"
  title: "Fight Club",
  
  // AÇIKLAMA için 3 alternatif (hangisi doluysa kullan):
  description: "An insomniac office worker...",  // Ya bu
  overview: "An insomniac office worker...",     // Ya da bu
  
  // GÖRSEL için 2 alternatif:
  coverUrl: "https://image.tmdb.org/t/p/w500/...",  // Ya bu
  posterPath: "/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",  // Ya da bu
  
  // YIL için 2 alternatif:
  year: 1999,    // Ya bu (int)
  releaseDate: "1999-10-15",     // Ya da bu (string, ilk 4 karakter parse edilir)
  
  // DETAYLI ALANLAR (Film için):
  director: "David Fincher",
  cast: ["Brad Pitt", "Edward Norton", "Helena Bonham Carter"],
  genres: ["Drama", "Thriller"]
};

// API Çaðrýsý
const response = await api.post('/Content/ensure', ensureContentData);
console.log(response.data);  // { success: true, data: { contentId: 123 } }
```

```javascript
// ÖRNEK 2: Kitap Ekleme
const ensureBookData = {
  externalId: "jAUODAAAQBAJ",  // Google Books ID
  type: "book",
  title: "Harry Potter and the Philosopher's Stone",
  description: "Harry Potter has never even heard of Hogwarts...",
  coverUrl: "https://books.google.com/books/content?id=...",
  year: 1997,
  
  // DETAYLI ALANLAR (Kitap için):
  authors: ["J.K. Rowling"],
  pageCount: 223
};

const response = await api.post('/Content/ensure', ensureBookData);
```

### **Backend Response:**

#### Baþarýlý (Yeni Oluþturuldu):
```json
{
  "success": true,
  "message": "Icerik basariyla eklendi.",
  "data": {
    "contentId": 123
  },
  "errors": null
}
```

#### Baþarýlý (Zaten Mevcut):
```json
{
  "success": true,
  "message": "Icerik zaten mevcut.",
  "data": {
  "contentId": 45,
    "message": "Icerik zaten mevcut."
  },
  "errors": null
}
```

#### Hata:
```json
{
  "success": false,
  "message": "Icerik eklenirken bir hata olustu.",
  "data": null,
  "errors": ["Detailed error message here"]
}
```

---

## ?? **2. Get Content Reviews Endpoint**

### **URL:** `GET /api/Content/{contentId}/reviews?page=1`
### **Auth:** ? Not Required

### **Frontend Kullanýmý:**

```javascript
// Yorumlarý getir
const getReviews = async (contentId, page = 1) => {
  try {
    const response = await api.get(`/Content/${contentId}/reviews?page=${page}`);
  
    if (response.data.success) {
      const { items, totalCount, totalPages } = response.data.data;
      
      console.log(`Toplam ${totalCount} yorum, ${totalPages} sayfa`);
      
      items.forEach(review => {
        console.log(`${review.userName}: ${review.text}`);
      });
      
      return response.data.data;
 }
  } catch (error) {
    console.error('Yorumlar yüklenemedi:', error);
    return { items: [], totalCount: 0, totalPages: 0 };
  }
};

// Kullaným
const reviews = await getReviews(123, 1);
```

### **Backend Response:**

```json
{
  "success": true,
  "message": "Icerik yorumlari basariyla getirildi. Toplam 5 yorum.",
  "data": {
    "items": [
      {
    "id": 1,
"userId": 42,
    "userName": "musti",
        "userAvatarUrl": null,
        "text": "Harika bir film!",
        "createdAt": "2025-03-27T10:30:00Z",
        "updatedAt": null
      }
    ],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "errors": null
}
```

### **Content Yoksa:**

```json
{
  "success": true,
  "message": "Bu icerik henuz veritabaninda yok - yorum bulunmuyor.",
  "data": {
    "items": [],
    "totalCount": 0,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 0
},
  "errors": null
}
```

---

## ?? **3. Rate Content Endpoint**

### **URL:** `POST /api/Content/rate`
### **Auth:** ? Required

### **Frontend Kullanýmý:**

```javascript
const rateContent = async (contentId, score) => {
  try {
    const response = await api.post('/Content/rate', {
      contentId: contentId,  // Backend'in kendi database ID'si
      score: score   // 1-10 arasý
    });
    
    if (response.data.success) {
 console.log(response.data.message);  // "Puan basariyla kaydedildi."
      return response.data.data.score;
 }
  } catch (error) {
    console.error('Puanlama baþarýsýz:', error);
    throw error;
  }
};

// Kullaným
await rateContent(123, 9);
```

### **Backend Response:**

```json
{
  "success": true,
  "message": "Puan basariyla kaydedildi.",
"data": {
  "ratingId": 456,
    "score": 9,
    "isUpdate": false
  },
  "errors": null
}
```

---

## ?? **4. Add Review Endpoint**

### **URL:** `POST /api/Content/review`
### **Auth:** ? Required

### **Frontend Kullanýmý:**

```javascript
const addReview = async (contentId, reviewText) => {
  try {
    const response = await api.post('/Content/review', {
      contentId: contentId,
      text: reviewText
    });
  
    if (response.data.success) {
      console.log('Yorum eklendi:', response.data.data.reviewId);
return response.data.data;
    }
} catch (error) {
 if (error.response?.data?.message === "Bu icerik hakkinda zaten yorum yaptiniz. Yorumunuzu duzenleyebilirsiniz.") {
      console.log('Zaten yorum yapýlmýþ');
 }
    throw error;
  }
};

// Kullaným
await addReview(123, "Gerçekten harika bir filmdi!");
```

### **Backend Response:**

```json
{
  "success": true,
  "message": "Yorum basariyla kaydedildi.",
  "data": {
    "reviewId": 789
  },
"errors": null
}
```

---

## ?? **5. Get My Rating Endpoint**

### **URL:** `GET /api/Content/{contentId}/my-rating`
### **Auth:** ? Required

### **Frontend Kullanýmý:**

```javascript
const getMyRating = async (contentId) => {
  try {
    const response = await api.get(`/Content/${contentId}/my-rating`);
    
    if (response.data.success && response.data.data !== null) {
      return response.data.data.score;  // 1-10 arasý puan
  }
    return null;  // Henüz puan verilmemiþ
  } catch (error) {
    console.error('Puan getirilemedi:', error);
    return null;
  }
};

// Kullaným
const myScore = await getMyRating(123);
if (myScore) {
  console.log(`Bu içeriðe ${myScore} puan vermiþsiniz`);
} else {
  console.log('Henüz puan vermemiþsiniz');
}
```

### **Backend Response (Puan Varsa):**

```json
{
  "success": true,
  "message": "Puaniniz basariyla getirildi.",
  "data": {
    "score": 9,
    "createdAt": "2025-03-27T10:30:00Z",
    "updatedAt": null
  },
  "errors": null
}
```

### **Backend Response (Puan Yoksa):**

```json
{
  "success": true,
  "message": "Bu iceriye henuz puan vermediniz.",
  "data": null,
  "errors": null
}
```

---

## ?? **6. Get Content Details Endpoint**

### **URL:** `GET /api/Content/{id}`
### **Auth:** ? Not Required

### **Frontend Kullanýmý:**

```javascript
const getContentDetails = async (contentId) => {
  try {
    const response = await api.get(`/Content/${contentId}`);
    
    if (response.data.success) {
      const content = response.data.data;
      
      console.log('Baþlýk:', content.title);
   console.log('Yönetmen:', content.director);
      console.log('Oyuncular:', content.cast);
      console.log('Türler:', content.genres);
      console.log('Ortalama Puan:', content.averageRating);
      console.log('Puan Sayýsý:', content.ratingsCount);
    console.log('Yorum Sayýsý:', content.reviewsCount);
      
      // Kullanýcý giriþ yapmýþsa:
      console.log('Benim Puaným:', content.currentUserRating);
      console.log('Yorum Yaptým mý:', content.hasUserReviewed);
      
      return content;
    }
  } catch (error) {
    console.error('Ýçerik detaylarý getirilemedi:', error);
    return null;
  }
};
```

### **Backend Response:**

```json
{
  "success": true,
  "message": "Icerik detaylari basariyla getirildi.",
  "data": {
    "id": 123,
    "externalId": "550",
"type": "movie",
    "title": "Fight Club",
    "description": "An insomniac office worker...",
    "year": 1999,
    "coverUrl": "https://image.tmdb.org/t/p/w500/...",
    
    "director": "David Fincher",
    "cast": ["Brad Pitt", "Edward Norton", "Helena Bonham Carter"],
    "genres": ["Drama", "Thriller"],
    "authors": null,
    "pageCount": null,
    
    "averageRating": 8.7,
    "ratingsCount": 42,
    "reviewsCount": 15,
    "listAddCount": 28,
    
    "currentUserRating": 9,
    "hasUserReviewed": true,
    "userLibraryStatus": {
      "isInWatchedList": true,
      "isInToWatchList": false,
      "isInReadList": false,
      "isInToReadList": false,
   "customLists": ["Favorilerim", "Aksiyon Filmleri"]
    },
    
    "reviews": [
      {
        "id": 1,
        "userId": 42,
        "userName": "musti",
        "userAvatarUrl": null,
        "text": "Harika bir film!",
   "createdAt": "2025-03-27T10:30:00Z",
        "updatedAt": null
      }
  ]
  },
  "errors": null
}
```

---

## ?? **FRONTEND ÝÞ AKIÞI**

### **Film Detay Sayfasý Yüklendiðinde:**

```javascript
// ContentDetail.jsx

const loadMovieDetails = async (tmdbId) => {
  try {
    // 1. ADIM: TMDb'den film bilgilerini al
    const tmdbMovie = await searchMovies(tmdbId);
 
    // 2. ADIM: Backend'de content oluþtur veya var olaný getir
    const ensureResponse = await api.post('/Content/ensure', {
      externalId: tmdbMovie.id.toString(),
      type: 'movie',
      title: tmdbMovie.title,
      overview: tmdbMovie.overview,
   posterPath: tmdbMovie.poster_path,
      releaseDate: tmdbMovie.release_date,
      director: tmdbMovie.director,
  cast: tmdbMovie.cast?.slice(0, 10).map(c => c.name),
      genres: tmdbMovie.genres?.map(g => g.name)
    });
    
    const contentId = ensureResponse.data.data.contentId;
    console.log('Content ID:', contentId);
    
    // 3. ADIM: Content detaylarýný getir
    const detailsResponse = await api.get(`/Content/${contentId}`);
    const contentDetails = detailsResponse.data.data;
    
    // 4. ADIM: Kullanýcýnýn puanýný getir (giriþ yapmýþsa)
    if (isAuthenticated) {
    const ratingResponse = await api.get(`/Content/${contentId}/my-rating`);
      setUserRating(ratingResponse.data.data?.score || null);
    }
    
    // 5. ADIM: Yorumlarý getir
    const reviewsResponse = await api.get(`/Content/${contentId}/reviews?page=1`);
    setReviews(reviewsResponse.data.data.items);
    
 // State'leri güncelle
    setContent(contentDetails);
    setLoading(false);
  } catch (error) {
    console.error('Film detaylarý yüklenirken hata:', error);
    setError(error.message);
  }
};
```

### **Puan Verme:**

```javascript
const handleRating = async (newScore) => {
  try {
    // Önce content var mý kontrol et (ensure ile)
    const ensureResponse = await api.post('/Content/ensure', {
      externalId: movieData.id.toString(),
      type: 'movie',
      title: movieData.title,
 // ... diðer alanlar
    });
    
    const contentId = ensureResponse.data.data.contentId;
    
    // Þimdi puan ver
    await api.post('/Content/rate', {
      contentId: contentId,
 score: newScore
    });
    
    setUserRating(newScore);
    toast.success('Puan kaydedildi!');
  } catch (error) {
    console.error('Puanlama hatasý:', error);
    toast.error('Puan kaydedilemedi');
  }
};
```

### **Yorum Ekleme:**

```javascript
const handleSubmitReview = async (reviewText) => {
  try {
    const response = await api.post('/Content/review', {
      contentId: content.id,
      text: reviewText
    });
    
    if (response.data.success) {
      toast.success('Yorum eklendi!');
      // Yorumlarý yeniden yükle
 await loadReviews(content.id);
    }
  } catch (error) {
    if (error.response?.data?.message?.includes('zaten yorum yaptiniz')) {
  toast.warning('Bu içerik hakkýnda zaten yorum yaptýnýz');
    } else {
    toast.error('Yorum eklenemedi');
    }
  }
};
```

---

## ? **KONTROL LÝSTESÝ**

### Backend:
- [x] `EnsureContent` endpoint'i güncellendi
- [x] `PosterPath`, `ReleaseDate`, `Overview` alanlarý destekleniyor
- [x] Alternatif alan isimleri handle ediliyor
- [x] Detaylý logging eklendi
- [x] `GetContentReviews` endpoint'i mevcut ve çalýþýyor
- [x] `RateContent` validation var
- [x] `AddReview` endpoint'i çalýþýyor
- [x] `GetMyRating` content yoksa 200 OK döndürüyor
- [x] Build baþarýlý ??

### Frontend Yapýlacaklar:
- [ ] `ensureContent` çaðrýsýný puan vermeden ÖNCE yap
- [ ] `ensureContent` çaðrýsýný yorum eklemeden ÖNCE yap
- [ ] `contentId`'yi `ensureContent` response'undan al
- [ ] Hata mesajlarýný user-friendly göster
- [ ] Loading states ekle
- [ ] Toast notifications ekle

---

## ?? **Test Senaryolarý**

### Test 1: Film Ekle ve Puan Ver
```bash
# 1. Ensure Content
POST /api/Content/ensure
{
  "externalId": "550",
  "type": "movie",
  "title": "Fight Club",
  "posterPath": "/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",
  "releaseDate": "1999-10-15",
  "overview": "An insomniac office worker..."
}

Response: { "success": true, "data": { "contentId": 123 } }

# 2. Rate Content
POST /api/Content/rate
{
  "contentId": 123,
  "score": 9
}

Response: { "success": true, "message": "Puan basariyla kaydedildi." }
```

### Test 2: Yorumlarý Getir
```bash
GET /api/Content/123/reviews?page=1

Response: {
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

## ?? **Sonuç**

? Tüm endpoint'ler hazýr ve çalýþýyor  
? Frontend ile tam uyumlu  
? Detaylý logging mevcut  
? Hata handling yapýlmýþ  
? Alternatif alan isimleri destekleniyor  
? Content yoksa graceful handling  

**Backend'i yeniden baþlatýn ve test edin!** ??

---

**Oluþturulma Tarihi:** 2024-12-04  
**Durum:** ? HAZIR
