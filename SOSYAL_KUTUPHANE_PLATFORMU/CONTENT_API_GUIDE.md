# Content API - Frontend Entegrasyon Rehberi

## ? YENÝ ENDPOINT EKLENDÝ!

### Yorum Sayfalama Endpoint'i
```
GET /api/content/{contentId}/yorumlar?pageNumber=1&pageSize=10
```

---

## ?? TÜM CONTENT ENDPOINT'LERÝ

### 1. Ýçerik Detaylarý (Yorumlar Dahil)
```
GET /api/content/{id}
Authorization: Bearer {token} (opsiyonel - anonim kullanýcý için userLibraryStatus null)
```

**Response:**
```json
{
  "success": true,
  "message": "Icerik detaylari basariyla getirildi.",
  "data": {
    "id": 1,
    "externalId": "550",
"type": "movie",
    "title": "Fight Club",
    "description": "Bir ofis calisaninin hikayesi...",
    "year": 1999,
 "coverUrl": "https://image.tmdb.org/t/p/w500/abc.jpg",
    "averageRating": 8.5,
    "ratingsCount": 150,
    "reviewsCount": 45,
    "listAddCount": 200,
  "currentUserRating": 9,
    "hasUserReviewed": true,
    "userLibraryStatus": {
  "isInWatchedList": true,
      "isInToWatchList": false,
  "isInReadList": false,
      "isInToReadList": false,
      "customLists": ["Favorilerim", "En Iyiler"]
    },
    "reviews": [
      {
      "id": 1,
     "userId": 5,
        "userName": "testuser",
  "userAvatarUrl": "https://example.com/avatar.jpg",
        "text": "Harika bir film!",
      "createdAt": "2024-01-15T10:30:00Z",
        "updatedAt": null
 }
   // ... ilk 20 yorum (varsayilan olarak tum yorumlar)
    ]
  }
}
```

---

### 2. Sadece Yorumlarý Getir (Sayfalanmýþ) ? YENÝ!
```
GET /api/content/{contentId}/yorumlar?pageNumber=1&pageSize=10
```

**Query Parameters:**
- `pageNumber` (default: 1) - Sayfa numarasi
- `pageSize` (default: 10, max: 100) - Sayfa basina yorum sayisi

**Success Response (200):**
```json
{
  "success": true,
  "message": "Icerik yorumlari basariyla getirildi. Toplam 45 yorum.",
  "data": {
    "items": [
      {
        "id": 1,
        "userId": 5,
     "userName": "testuser",
        "userAvatarUrl": "https://example.com/avatar.jpg",
        "text": "Harika bir film! Kesinlikle izlenmeli.",
        "createdAt": "2024-01-15T10:30:00Z",
        "updatedAt": "2024-01-16T12:00:00Z"
      },
      {
        "id": 2,
  "userId": 8,
 "userName": "filmSever",
        "userAvatarUrl": null,
        "text": "Cok begendim. Tavsiye ederim.",
        "createdAt": "2024-01-14T15:20:00Z",
        "updatedAt": null
  }
    ],
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 45,
    "hasPrevious": false,
"hasNext": true
  }
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Icerik bulunamadi.",
  "errors": null
}
```

---

### 3. Puan Verme (Rating)
```
POST /api/content/rate
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "contentId": 1,
  "score": 8
}
```

**Validation:**
- `contentId`: Zorunlu (integer)
- `score`: Zorunlu, 1-10 arasý (integer)

**Success Response (200) - Yeni Puan:**
```json
{
  "success": true,
  "message": "Puan basariyla kaydedildi.",
  "data": {
    "ratingId": 5,
    "score": 8,
    "isUpdate": false
  }
}
```

**Success Response (200) - Puan Güncelleme:**
```json
{
  "success": true,
  "message": "Puan guncellendi.",
  "data": {
    "ratingId": 5,
    "score": 9,
    "isUpdate": true
  }
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Icerik bulunamadi.",
  "errors": null
}
```

---

### 4. Yorum Ekleme
```
POST /api/content/review
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "contentId": 1,
  "text": "Harika bir film! Kesinlikle izlenmeli."
}
```

**Validation:**
- `contentId`: Zorunlu (integer)
- `text`: Zorunlu, 10-2000 karakter arasý (string)

**Success Response (200):**
```json
{
  "success": true,
  "message": "Yorum basariyla kaydedildi.",
  "data": {
    "reviewId": 12
  }
}
```

**Error Response (400) - Duplicate:**
```json
{
  "success": false,
  "message": "Bu icerik hakkinda zaten yorum yaptiniz. Yorumunuzu duzenleyebilirsiniz.",
  "errors": null
}
```

---

### 5. Kullanýcýnýn Puanýný Getir
```
GET /api/content/{contentId}/my-rating
Authorization: Bearer {token}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Puaniniz basariyla getirildi.",
  "data": {
    "score": 8,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-16T12:00:00Z"
  }
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Bu iceriðe henuz puan vermediniz.",
  "errors": null
}
```

---

### 6. Kullanýcýnýn Yorumunu Getir
```
GET /api/content/{contentId}/my-review
Authorization: Bearer {token}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Yorumunuz basariyla getirildi.",
  "data": {
 "id": 12,
    "text": "Harika bir film!",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": null
  }
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Bu icerik hakkinda henuz yorum yapmadiniz.",
  "errors": null
}
```

---

### 7. Yorum Güncelleme
```
PUT /api/content/review/{reviewId}
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "contentId": 1,
  "text": "Guncellenmis yorum metni."
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Yorum basariyla guncellendi.",
  "data": null
}
```

**Error Response (403):**
```json
{
  "success": false,
  "message": "Forbidden",
  "errors": null
}
```
_Not: Sadece kendi yorumunu guncelleyebilir._

---

### 8. Yorum Silme
```
DELETE /api/content/review/{reviewId}
Authorization: Bearer {token}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Yorum basariyla silindi.",
  "data": null
}
```

---

## ?? Frontend Kullaným Örnekleri

### React + Axios

#### 1. Ýçerik Detaylarýný Getir:
```javascript
import axios from 'axios';

const getContentDetails = async (contentId) => {
  try {
    const response = await axios.get(
      `https://localhost:7297/api/content/${contentId}`,
      {
        headers: {
    Authorization: `Bearer ${localStorage.getItem('authToken')}`
        }
      }
    );
    
    if (response.data.success) {
  return response.data.data;
    }
  } catch (error) {
    console.error('Icerik detay hatasi:', error);
    throw error;
  }
};
```

#### 2. Yorumlarý Sayfalý Getir (YENÝ):
```javascript
const getContentReviews = async (contentId, pageNumber = 1, pageSize = 10) => {
  try {
    const response = await axios.get(
      `https://localhost:7297/api/content/${contentId}/yorumlar`,
      {
        params: { pageNumber, pageSize }
      }
    );
    
    if (response.data.success) {
      const { items, currentPage, totalPages, hasNext } = response.data.data;
return { reviews: items, currentPage, totalPages, hasNext };
    }
  } catch (error) {
    console.error('Yorum listesi hatasi:', error);
    throw error;
  }
};

// Kullanim:
const { reviews, currentPage, totalPages, hasNext } = await getContentReviews(1, 1, 10);
```

#### 3. Puan Ver:
```javascript
const rateContent = async (contentId, score) => {
  try {
    const response = await axios.post(
      'https://localhost:7297/api/content/rate',
      {
  contentId: contentId,
        score: score // 1-10 arasi
      },
      {
        headers: {
Authorization: `Bearer ${localStorage.getItem('authToken')}`,
          'Content-Type': 'application/json'
  }
      }
    );
    
    if (response.data.success) {
      const { isUpdate } = response.data.data;
      console.log(isUpdate ? 'Puan guncellendi' : 'Yeni puan verildi');
      return response.data.data;
    }
  } catch (error) {
    console.error('Puanlama hatasi:', error);
    throw error;
  }
};
```

#### 4. Yorum Ekle:
```javascript
const addReview = async (contentId, text) => {
  try {
    const response = await axios.post(
      'https://localhost:7297/api/content/review',
      {
     contentId: contentId,
  text: text
      },
  {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('authToken')}`,
  'Content-Type': 'application/json'
}
      }
    );
    
 if (response.data.success) {
  return response.data.data.reviewId;
    }
  } catch (error) {
    if (error.response?.status === 400) {
      alert('Bu icerik hakkinda zaten yorum yaptiniz.');
    }
    throw error;
  }
};
```

---

## ?? UI Component Örnekleri

### Rating Component:
```jsx
import { useState, useEffect } from 'react';
import axios from 'axios';

const RatingComponent = ({ contentId }) => {
  const [userRating, setUserRating] = useState(null);
  const [hoveredStar, setHoveredStar] = useState(0);

  useEffect(() => {
    // Kullanicinin var olan puanini getir
    const fetchUserRating = async () => {
      try {
        const response = await axios.get(
          `https://localhost:7297/api/content/${contentId}/my-rating`,
  {
            headers: {
   Authorization: `Bearer ${localStorage.getItem('authToken')}`
  }
          }
        );
        if (response.data.success) {
       setUserRating(response.data.data.score);
     }
    } catch (error) {
 // Henuz puan verilmemis
        setUserRating(null);
   }
    };

    fetchUserRating();
  }, [contentId]);

  const handleRate = async (score) => {
    try {
const response = await axios.post(
        'https://localhost:7297/api/content/rate',
        { contentId, score },
    {
          headers: {
   Authorization: `Bearer ${localStorage.getItem('authToken')}`
      }
        }
      );
      
  if (response.data.success) {
        setUserRating(score);
      }
    } catch (error) {
      console.error('Puanlama hatasi:', error);
    }
  };

  return (
    <div className="rating">
      <p>Puaniniz: {userRating || 'Henuz puan vermediniz'}</p>
      <div className="stars">
        {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((star) => (
     <span
     key={star}
    className={star <= (hoveredStar || userRating) ? 'filled' : ''}
        onMouseEnter={() => setHoveredStar(star)}
            onMouseLeave={() => setHoveredStar(0)}
            onClick={() => handleRate(star)}
        >
     ?
 </span>
))}
      </div>
    </div>
  );
};
```

### Reviews List Component (Sayfalama ile):
```jsx
import { useState, useEffect } from 'react';
import axios from 'axios';

const ReviewsList = ({ contentId }) => {
  const [reviews, setReviews] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [hasNext, setHasNext] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchReviews(currentPage);
  }, [contentId, currentPage]);

  const fetchReviews = async (page) => {
    setLoading(true);
    try {
      const response = await axios.get(
        `https://localhost:7297/api/content/${contentId}/yorumlar`,
        {
    params: { pageNumber: page, pageSize: 10 }
  }
 );

      if (response.data.success) {
        const { items, totalPages, hasNext } = response.data.data;
        setReviews(items);
        setTotalPages(totalPages);
        setHasNext(hasNext);
      }
    } catch (error) {
      console.error('Yorum listesi hatasi:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="reviews-list">
      <h3>Yorumlar</h3>
      
      {loading ? (
        <p>Yukleniyor...</p>
      ) : (
      <>
          {reviews.map((review) => (
    <div key={review.id} className="review-item">
      <div className="review-header">
     <img 
      src={review.userAvatarUrl || '/default-avatar.png'} 
     alt={review.userName} 
      />
     <span>{review.userName}</span>
     <span className="date">
         {new Date(review.createdAt).toLocaleDateString('tr-TR')}
  </span>
              </div>
    <p>{review.text}</p>
   </div>
 ))}

{/* Sayfalama */}
     <div className="pagination">
            <button 
    onClick={() => setCurrentPage(currentPage - 1)}
          disabled={currentPage === 1}
   >
       Onceki
            </button>
      
      <span>Sayfa {currentPage} / {totalPages}</span>
      
 <button 
       onClick={() => setCurrentPage(currentPage + 1)}
     disabled={!hasNext}
            >
         Sonraki
    </button>
          </div>
        </>
  )}
    </div>
  );
};
```

---

## ?? Önemli Notlar

### 1. Endpoint Farklari:
- **GET /api/content/{id}** ? Tüm içerik detaylarý + ilk yorumlar (sayfalama YOK)
- **GET /api/content/{id}/yorumlar** ? Sadece yorumlar (sayfalama VAR) ? YENÝ

### 2. Kullaným Önerisi:
```javascript
// Sayfa ilk yuklendiginde
const contentDetails = await getContentDetails(contentId);
// -> Bu, genel bilgileri + ilk 20 yorumu getirir

// Daha fazla yorum icin
const moreReviews = await getContentReviews(contentId, 2, 10);
// -> Sayfa 2'deki 10 yorumu getirir
```

### 3. Authorization:
- `/api/content/{id}` ? Token opsiyonel (anonim kullanýcý için `userLibraryStatus` null)
- `/api/content/rate` ? Token zorunlu
- `/api/content/review` ? Token zorunlu

---

Son Guncelleme: 2024
Durum: ? YORUM SAYFALAMA EKLENDÝ - FRONTEND ENTEGRASYONU HAZIR
