using System.Text.Json;
using System.Text.Json.Serialization;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public class GoogleBooksService : IGoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public GoogleBooksService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExternalApis:GoogleBooks:BaseUrl"]
                       ?? "https://www.googleapis.com/books/v1";
        }

        public async Task<List<SearchResultItemDto>> SearchBooksAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultItemDto>();

            var url = $"{_baseUrl}/volumes?q={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<GoogleBooksSearchResponse>(json, options);

            var results = new List<SearchResultItemDto>();

            if (data?.Items != null)
            {
                foreach (var item in data.Items)
                {
                    var info = item.VolumeInfo;
                    if (info == null) continue;

                    int? year = null;
                    if (!string.IsNullOrEmpty(info.PublishedDate) && info.PublishedDate.Length >= 4)
                    {
                        if (int.TryParse(info.PublishedDate[..4], out var y))
                            year = y;
                    }

                    string? coverUrl = info.ImageLinks?.Thumbnail;

                    // PROJE METNI GEREKSINIMI: Yazarlar ve sayfa sayisi
                    var authors = info.Authors ?? new List<string>();
                    var pageCount = info.PageCount;

                    results.Add(new SearchResultItemDto
                    {
                        ExternalId = item.Id ?? "",
                        Type = "book",
                        Title = info.Title ?? "İsimsiz Kitap",
                        Description = info.Description,
                        Year = year,
                        CoverUrl = coverUrl,
                        Authors = authors,
                        PageCount = pageCount
                    });
                }
            }

            return results;
        }

        // Google Books response modelleri
        private class GoogleBooksSearchResponse
        {
            [JsonPropertyName("items")]
            public List<GoogleBookItem> Items { get; set; } = new();
        }

        private class GoogleBookItem
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("volumeInfo")]
            public VolumeInfo? VolumeInfo { get; set; }
        }

        private class VolumeInfo
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("publishedDate")]
            public string? PublishedDate { get; set; }

            [JsonPropertyName("imageLinks")]
            public ImageLinks? ImageLinks { get; set; }

            // PROJE METNI GEREKSINIMLERI
            [JsonPropertyName("authors")]
            public List<string>? Authors { get; set; }

            [JsonPropertyName("pageCount")]
            public int? PageCount { get; set; }
        }

        private class ImageLinks
        {
            [JsonPropertyName("thumbnail")]
            public string? Thumbnail { get; set; }
        }
    }
}
