# ?? USER SEARCH ENDPOINT - YENÝ ÖZELLÝK

## ?? **Eklenen Özellik**

### **Yeni Endpoint:**
```
GET /api/User/search?query={searchTerm}
```

Kullanýcýlar artýk diðer kullanýcýlarý **username**'e göre arayabilir.

---

## ?? **Endpoint Detaylarý**

### **URL:** `GET /api/User/search`
### **Auth:** ? Required (Bearer Token)
### **Query Params:** `query` (string, required)

### **Request:**
```http
GET /api/User/search?query=ahmet
Authorization: Bearer {token}
```

### **Response (Baþarýlý):**
```json
{
  "success": true,
  "message": "5 kullanýcý bulundu.",
  "data": [
    {
      "id": 2,
"userName": "ahmet",
      "avatarUrl": null,
      "biography": "Film tutkunuyum",
      "followersCount": 42,
   "followingCount": 18,
      "isFollowing": false
    },
    {
      "id": 5,
      "userName": "ahmetyilmaz",
    "avatarUrl": "https://...",
      "biography": null,
      "followersCount": 10,
   "followingCount": 5,
      "isFollowing": true
    }
  ],
  "errors": null
}
```

### **Response (Query Boþ):**
```json
{
  "success": false,
  "message": "Arama sorgusu boþ olamaz.",
"data": null,
  "errors": null
}
```

### **Response (Kullanýcý Bulunamadý):**
```json
{
  "success": true,
  "message": "0 kullanýcý bulundu.",
  "data": [],
  "errors": null
}
```

---

## ?? **Backend Implementasyonu**

### **UserController.cs:**
```csharp
[HttpGet("search")]
[Authorize]
public async Task<ActionResult<ApiResponse<List<FollowUserDto>>>> SearchUsers([FromQuery] string query)
{
    if (string.IsNullOrWhiteSpace(query))
        return BadRequest(ApiResponse<List<FollowUserDto>>.FailResponse("Arama sorgusu boþ olamaz."));

    var currentUserId = GetCurrentUserId();

  // Kullanýcýlarý ara
    var users = await _context.Users
     .Where(u => u.UserName.Contains(query) && u.Id != currentUserId) // Kendini hariç tut
        .Take(20)
        .ToListAsync();

    // Giriþ yapan kullanýcýnýn takip ettiði kiþileri al
    var currentUserFollowingIds = await _context.UserFollowers
    .Where(uf => uf.FollowerId == currentUserId)
        .Select(uf => uf.FollowingId)
   .ToListAsync();

    var result = users.Select(u => new FollowUserDto
    {
Id = u.Id,
     UserName = u.UserName,
        AvatarUrl = u.AvatarUrl,
  Biography = u.Biography,
      FollowersCount = u.FollowersCount,
      FollowingCount = u.FollowingCount,
        IsFollowing = currentUserFollowingIds.Contains(u.Id)
    }).ToList();

    return Ok(ApiResponse<List<FollowUserDto>>.SuccessResponse(
        result,
        $"{result.Count} kullanýcý bulundu."));
}
```

---

## ? **Özellikler**

### **1. Case-Insensitive Search**
```csharp
.Where(u => u.UserName.Contains(query))
```
- `"ahmet"` ? `"Ahmet"`, `"ahmetyilmaz"`, `"mehmetahmet"` bulur
- SQL Server `LIKE '%ahmet%'` sorgusuna çevrilir

### **2. Kendini Hariç Tutma**
```csharp
.Where(u => u.Id != currentUserId)
```
- Giriþ yapan kullanýcý kendi adýný arasa bile sonuçlarda çýkmaz

### **3. Follow Status**
```csharp
IsFollowing = currentUserFollowingIds.Contains(u.Id)
```
- Frontend'de "Takip Et" veya "Takiptesin" butonu göstermek için

### **4. Limit**
```csharp
.Take(20)
```
- Maksimum 20 sonuç döndürür
- Performance için

---

## ?? **Test Senaryolarý**

### **Test 1: Kullanýcý Ara**

```http
GET /api/User/search?query=ahmet
Authorization: Bearer {token}
```

**Beklenen:** `200 OK + Kullanýcý listesi`

---

### **Test 2: Boþ Query**

```http
GET /api/User/search?query=
Authorization: Bearer {token}
```

**Beklenen:** `400 Bad Request + "Arama sorgusu boþ olamaz."`

---

### **Test 3: Hiç Sonuç Yok**

```http
GET /api/User/search?query=xxxxxxxxxx
Authorization: Bearer {token}
```

**Beklenen:** `200 OK + []` (boþ array)

---

### **Test 4: Kendi Adýný Ara**

```http
GET /api/User/search?query=musti
Authorization: Bearer {token} (musti kullanýcýsý)
```

**Beklenen:** `200 OK + []` (kendisi sonuçlarda çýkmaz)

---

## ?? **Frontend Kullanýmý**

### **api.js:**
```javascript
// api.js
export const searchUsers = async (query) => {
  const response = await axios.get(
    `/User/search`,
    {
    params: { query },
      headers: { Authorization: `Bearer ${getToken()}` }
    }
  );
  return response.data;
};
```

### **UserSearch.jsx:**
```javascript
// UserSearch.jsx
import { useState } from 'react';
import { searchUsers } from '../services/api';

const UserSearch = () => {
  const [query, setQuery] = useState('');
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleSearch = async (e) => {
    const searchTerm = e.target.value;
    setQuery(searchTerm);

    if (searchTerm.length < 2) {
      setUsers([]);
      return;
    }

    setLoading(true);
    try {
    const response = await searchUsers(searchTerm);
      if (response.success) {
        setUsers(response.data);
      }
    } catch (error) {
      console.error('Search error:', error);
 } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <input
        type="text"
        value={query}
        onChange={handleSearch}
  placeholder="Kullanýcý ara..."
      />

      {loading && <p>Aranýyor...</p>}

      <div>
        {users.map(user => (
          <div key={user.id}>
        <img src={user.avatarUrl || '/default-avatar.png'} alt={user.userName} />
            <h3>{user.userName}</h3>
         <p>{user.biography}</p>
       <p>{user.followersCount} takipçi</p>
    
  {user.isFollowing ? (
              <button>Takiptesin</button>
   ) : (
    <button>Takip Et</button>
 )}
        </div>
  ))}
    </div>

      {users.length === 0 && query.length >= 2 && !loading && (
        <p>Kullanýcý bulunamadý.</p>
      )}
  </div>
  );
};

export default UserSearch;
```

---

## ?? **UI Önerileri**

### **1. Debounce ile Arama**
```javascript
// 500ms bekle, sonra ara
import { useDebounce } from 'use-debounce';

const [debouncedQuery] = useDebounce(query, 500);

useEffect(() => {
  if (debouncedQuery.length >= 2) {
    handleSearch(debouncedQuery);
  }
}, [debouncedQuery]);
```

### **2. Loading State**
```jsx
{loading && (
  <div className="spinner">
    <CircularProgress />
  </div>
)}
```

### **3. Empty State**
```jsx
{users.length === 0 && query.length >= 2 && (
  <div className="empty-state">
    <SearchIcon />
    <p>"{query}" için sonuç bulunamadý</p>
  </div>
)}
```

---

## ?? **Ýyileþtirme Önerileri (Gelecek)**

### **1. Full-Text Search**
```csharp
// SQL Server Full-Text Index kullanarak:
.Where(u => EF.Functions.FreeText(u.UserName, query))
```

### **2. Fuzzy Search**
```csharp
// Typo'lara toleranslý arama:
// "ahemt" ? "ahmet" bulabilsin
```

### **3. Biography'de de Ara**
```csharp
.Where(u => u.UserName.Contains(query) || u.Biography.Contains(query))
```

### **4. Pagination**
```csharp
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize)
```

---

## ?? **Response DTO**

### **FollowUserDto:**
```csharp
public class FollowUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
 public string? Biography { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }  // Giriþ yapan kullanýcý bu kiþiyi takip ediyor mu?
}
```

---

## ? **Sonuç**

### **Yapýlan Deðiþiklikler:**
- ? `GET /api/User/search` endpoint'i eklendi
- ? Username'e göre arama
- ? Kendini hariç tutma
- ? Follow status kontrolü
- ? 20 sonuç limiti
- ? Build baþarýlý
- ? GitHub'a push edildi

### **Þimdi Yapýlacaklar:**
1. ? Backend'i yeniden baþlat
2. ? Frontend'de `UserSearch` komponenti oluþtur
3. ? `api.js`'e `searchUsers` fonksiyonu ekle
4. ? Test et

---

**Oluþturulma Tarihi:** 2024-12-04  
**Commit Hash:** ed64968  
**Durum:** ? TAMAMLANDI

**ARTIK KULLANICILAR DÝÐER KULLANICILARI ARAYABÝLÝR!** ??
