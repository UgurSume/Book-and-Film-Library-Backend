# ?? DETAYLI F ÝLTRELEME VE KULLANICI DEÐERLENDÝRME SÝSTEMÝ

## ? **Eklenen Özellikler**

### **1. Kullanýcý Deðerlendirme Endpoint'i**
```
GET /api/Content/{contentId}/average-rating
```

Platform kullanýcýlarýnýn verdiði ortalama puaný gösterir (API puaný deðil!).

### **2. Detaylý Filtreleme Endpoint'i**
```
POST /api/Search/filter
```

Türe, puana, yýla, yönetmene, yazara göre filtreleme yapabilirsiniz.

---

## ?? **1. KULLANICI DEÐERLENDÝRME**

### **Endpoint:** `GET /api/Content/{contentId}/average-rating`

**Amaç:** API'den gelen puan yerine, platformunuzdaki kullanýcýlarýn verdiði ortalama puaný gösterir.

### **Request:**
```http
GET /api/Content/123/average-rating
```

### **Response:**
```json
{
  "success": true,
  "message": "Ortalama puan baþarýyla getirildi.",
  "data": {
    "averageRating": 8.5,
    "ratingsCount": 42
},
  "errors": null
}
```

### **Response (Puan Yok):**
```json
{
  "success": true,
  "message": "Bu icerik henuz puanlanmamis.",
  "data": {
    "averageRating": 0.0,
    "ratingsCount": 0
  },
  "errors": null
}
```

---

## ?? **2. DETAYLI FÝLTRELEME SÝSTEMÝ**

### **Endpoint:** `POST /api/Search/filter`

**Amaç:** Platform içindeki içeriklerde geliþmiþ arama ve filtreleme.

### **Filtre Seçenekleri:**

| Filtre | Tip | Açýklama | Örnek |
|--------|-----|----------|-------|
| `type` | string | Ýçerik tipi | `"movie"` veya `"book"` |
| `searchTerm` | string | Baþlýkta arama | `"Inception"` |
| `genres` | string[] | Türler | `["Action", "Sci-Fi"]` |
| `director` | string | Yönetmen | `"Christopher Nolan"` |
| `author` | string | Yazar | `"J.K. Rowling"` |
| `minRating` | double | Minimum puan | `7.5` |
| `maxRating` | double | Maksimum puan | `10.0` |
| `minYear` | int | Minimum yýl | `2000` |
| `maxYear` | int | Maksimum yýl | `2024` |
| `sortBy` | string | Sýralama | `"title"`, `"year"`, `"rating"`, `"created"` |
| `sortDescending` | bool | Azalan sýralama | `true` |
| `pageNumber` | int | Sayfa numarasý | `1` |
| `pageSize` | int | Sayfa boyutu | `20` |

---

### **Request Örnekleri:**

#### **Örnek 1: Action Filmlerini Ara (2010 sonrasý, 8+ puan)**
```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "movie",
  "genres": ["Action"],
  "minYear": 2010,
  "minRating": 8.0,
  "sortBy": "rating",
  "sortDescending": true,
  "pageNumber": 1,
  "pageSize": 20
}
```

#### **Örnek 2: Harry Potter Kitaplarýný Ara**
```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "book",
  "searchTerm": "Harry Potter",
  "author": "J.K. Rowling",
  "sortBy": "year",
  "sortDescending": false,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### **Örnek 3: Christopher Nolan Filmlerini Ara**
```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "movie",
  "director": "Christopher Nolan",
  "minRating": 7.0,
  "sortBy": "rating",
  "sortDescending": true,
  "pageNumber": 1,
  "pageSize": 20
}
```

#### **Örnek 4: 90'lar Dramalarýný Ara**
```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "movie",
  "genres": ["Drama"],
  "minYear": 1990,
  "maxYear": 1999,
  "minRating": 7.5,
  "sortBy": "year",
  "sortDescending": true,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

### **Response:**
```json
{
  "success": true,
  "message": "15 icerik bulundu.",
  "data": {
    "items": [
      {
        "id": 123,
        "externalId": "550",
        "type": "movie",
        "title": "Fight Club",
        "year": 1999,
        "coverUrl": "https://...",
  "director": "David Fincher",
        "genres": ["Drama"],
        "authors": null,
        "pageCount": null,
        "averageRating": 8.8,
"ratingsCount": 156,
     "reviewsCount": 42,
   "listAddCount": 89,
        "createdAt": "2024-12-01T10:00:00Z"
      }
    ],
    "totalCount": 15,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "errors": null
}
```

---

## ?? **Frontend Kullanýmý**

### **1. Kullanýcý Deðerlendirmesi Gösterme**

```javascript
// api.js
export const getAverageRating = async (contentId) => {
  const response = await axios.get(`/Content/${contentId}/average-rating`);
  return response.data;
};

// ContentDetail.jsx
const [userRating, setUserRating] = useState(null);

useEffect(() => {
  const fetchUserRating = async () => {
    const response = await api.getAverageRating(contentId);
    if (response.success) {
      setUserRating(response.data);
    }
  };
  fetchUserRating();
}, [contentId]);

// Render
<div>
  <h3>Kullanýcý Puaný</h3>
  <div className="rating">
    <Star filled={userRating?.averageRating >= 1} />
    <span>{userRating?.averageRating || 'N/A'}</span>
    <span>({userRating?.ratingsCount} deðerlendirme)</span>
  </div>
</div>

// TMDb puaný ile karþýlaþtýrma
<div>
  <div>
    <span>TMDb:</span> {tmdbRating}
  </div>
<div>
    <span>Kullanýcýlarýmýz:</span> {userRating?.averageRating || 'Henüz yok'}
  </div>
</div>
```

---

### **2. Filtreleme Kullanýmý**

```javascript
// api.js
export const filterContents = async (filters) => {
  const response = await axios.post('/Search/filter', filters);
  return response.data;
};

// FilterPage.jsx
const [filters, setFilters] = useState({
  type: 'movie',
  genres: [],
  minRating: 7.0,
  minYear: 2000,
  sortBy: 'rating',
  sortDescending: true,
  pageNumber: 1,
  pageSize: 20
});

const [results, setResults] = useState([]);

const handleFilter = async () => {
  const response = await api.filterContents(filters);
  if (response.success) {
    setResults(response.data.items);
  }
};

// Render
<div className="filters">
  <select 
    value={filters.type} 
    onChange={(e) => setFilters({...filters, type: e.target.value})}
  >
    <option value="">Tümü</option>
    <option value="movie">Film</option>
    <option value="book">Kitap</option>
  </select>

  <input
    type="number"
    placeholder="Minimum Puan"
    value={filters.minRating}
    onChange={(e) => setFilters({...filters, minRating: parseFloat(e.target.value)})}
  />

  <input
    type="number"
    placeholder="Minimum Yýl"
  value={filters.minYear}
    onChange={(e) => setFilters({...filters, minYear: parseInt(e.target.value)})}
  />

  <MultiSelect
    options={['Action', 'Drama', 'Comedy', 'Sci-Fi', 'Thriller']}
    value={filters.genres}
    onChange={(genres) => setFilters({...filters, genres})}
  />

  <button onClick={handleFilter}>Filtrele</button>
</div>

<div className="results">
  {results.map(content => (
    <ContentCard key={content.id} content={content} />
  ))}
</div>
```

---

## ?? **Kullaným Senaryolarý**

### **Senaryo 1: "En Ýyi 90'lar Filmleri"**
```javascript
const filters = {
  type: 'movie',
  minYear: 1990,
  maxYear: 1999,
  minRating: 8.0,
  sortBy: 'rating',
  sortDescending: true,
  pageSize: 50
};
```

### **Senaryo 2: "Christopher Nolan'ýn En Ýyi Filmleri"**
```javascript
const filters = {
  type: 'movie',
  director: 'Christopher Nolan',
  sortBy: 'rating',
  sortDescending: true
};
```

### **Senaryo 3: "Yeni Çýkan Aksiyon Filmleri (2020+)"**
```javascript
const filters = {
  type: 'movie',
  genres: ['Action'],
  minYear: 2020,
  sortBy: 'year',
  sortDescending: true
};
```

### **Senaryo 4: "J.K. Rowling Kitaplarý"**
```javascript
const filters = {
  type: 'book',
  author: 'J.K. Rowling',
  sortBy: 'year',
  sortDescending: false
};
```

---

## ?? **Sýralama Seçenekleri**

| `sortBy` Deðeri | Açýklama |
|-----------------|----------|
| `"title"` | Baþlýða göre (A-Z veya Z-A) |
| `"year"` | Yýla göre (Eskiden yeniye veya yeniden eskiye) |
| `"rating"` | Puana göre (Yüksekten düþüðe veya düþükten yükseðe) |
| `"created"` | Eklenme tarihine göre (Son eklenenler veya ilk eklenenler) |

---

## ? **Yapýlan Deðiþiklikler**

### **Backend:**

#### **1. ContentController.cs:**
- ? `GET /api/Content/{contentId}/average-rating` endpoint'i eklendi
- ? Kullanýcýlarýn verdiði ortalama puaný hesaplýyor
- ? Puan sayýsýný döndürüyor

#### **2. SearchController.cs:**
- ? `POST /api/Search/filter` endpoint'i eklendi
- ? Türe, puana, yýla, yönetmene, yazara göre filtreleme
- ? Sýralama desteði (title, year, rating, created)
- ? Pagination desteði

#### **3. ContentFilterRequest.cs:**
- ? `SearchTerm` - Baþlýk aramasý
- ? `Genres` - Tür listesi
- ? `Director` - Yönetmen filtresi
- ? `Author` - Yazar filtresi
- ? `MinYear` / `MaxYear` - Yýl aralýðý
- ? `SortBy` / `SortDescending` - Sýralama

#### **4. ContentSummaryDto.cs:**
- ? `Director` - Film yönetmeni
- ? `Genres` - Türler listesi
- ? `Authors` - Kitap yazarlarý
- ? `PageCount` - Sayfa sayýsý

#### **5. DiscoverController.cs:**
- ? Filter endpoint'i yeni field'larla uyumlu hale getirildi

---

## ?? **Test Senaryolarý**

### **Test 1: Kullanýcý Deðerlendirmesi**

```http
GET /api/Content/123/average-rating

Beklenen: 200 OK + { averageRating: 8.5, ratingsCount: 42 }
```

### **Test 2: Basit Filtreleme**

```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "movie",
  "minRating": 8.0,
  "pageNumber": 1,
  "pageSize": 10
}

Beklenen: 200 OK + 8+ puanlý filmler
```

### **Test 3: Karmaþýk Filtreleme**

```http
POST /api/Search/filter
Content-Type: application/json

{
  "type": "movie",
  "genres": ["Action", "Sci-Fi"],
  "minYear": 2010,
  "minRating": 7.5,
  "director": "Christopher Nolan",
  "sortBy": "rating",
  "sortDescending": true,
"pageNumber": 1,
  "pageSize": 20
}

Beklenen: 200 OK + Christopher Nolan'ýn 2010 sonrasý Action/Sci-Fi filmleri
```

---

## ? **Sonuç**

### **Eklenen Özellikler:**
- ? Kullanýcý deðerlendirme sistemi (API puaný deðil, kendi kullanýcýlarýnýzýn puaný)
- ? Detaylý filtreleme (Tür, puan, yýl, yönetmen, yazar)
- ? Esnek sýralama (Baþlýk, yýl, puan, eklenme tarihi)
- ? Pagination desteði
- ? JSON tür arama (Genres, Authors)

### **Þimdi Yapýlacaklar:**
1. ? Backend'i yeniden baþlat
2. ? Frontend'de filtreleme sayfasý oluþtur
3. ? Kullanýcý puanýný göster (API puaný yanýnda)
4. ? Test et

---

**Oluþturulma Tarihi:** 2024-12-04  
**Commit Hash:** 5c21f47  
**Durum:** ? TAMAMLANDI

**ARTIK DETAYLI FÝLTRELEME VE KULLANICI DEÐERLENDÝRME SÝSTEMÝ HAZIR!** ??
