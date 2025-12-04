using System.Text.Json;
using System.Text.Json.Serialization;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _imageBaseUrl;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            _apiKey = configuration["ExternalApis:Tmdb:ApiKey"]
                      ?? throw new Exception("TMDb ApiKey tanımlı değil.");
            _baseUrl = configuration["ExternalApis:Tmdb:BaseUrl"]
                       ?? "https://api.themoviedb.org/3";
            _imageBaseUrl = configuration["ExternalApis:Tmdb:ImageBaseUrl"]
                            ?? "https://image.tmdb.org/t/p/w500";
        }

        public async Task<List<SearchResultItemDto>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultItemDto>();

            var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&language=tr-TR";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<TmdbSearchResponse>(json, options);

            var results = new List<SearchResultItemDto>();

            if (data?.Results != null)
            {
                foreach (var movie in data.Results)
                {
                    // PROJE METNI GEREKSINIMI: Detayli bilgi cek
                    var detailedMovie = await GetMovieDetailsAsync(movie.Id);

                    int? year = null;
                    if (!string.IsNullOrEmpty(movie.ReleaseDate) && movie.ReleaseDate.Length >= 4)
                    {
                        if (int.TryParse(movie.ReleaseDate[..4], out var y))
                            year = y;
                    }

                    string? coverUrl = null;
                    if (!string.IsNullOrEmpty(movie.PosterPath))
                    {
                        coverUrl = _imageBaseUrl + movie.PosterPath;
                    }

                    results.Add(new SearchResultItemDto
                    {
                        ExternalId = movie.Id.ToString(),
                        Type = "movie",
                        Title = movie.Title ?? movie.OriginalTitle ?? "İsimsiz Film",
                        Description = movie.Overview,
                        Year = year,
                        CoverUrl = coverUrl,
                        Director = detailedMovie?.Director,
                        Cast = detailedMovie?.Cast,
                        Genres = detailedMovie?.Genres
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// Film detaylarini cek (yonetmen, oyuncular, turler)
        /// </summary>
        private async Task<MovieDetails?> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                // 1. Film detaylari (turler icin)
                var detailUrl = $"{_baseUrl}/movie/{movieId}?api_key={_apiKey}&language=tr-TR";
                var detailResponse = await _httpClient.GetAsync(detailUrl);
                detailResponse.EnsureSuccessStatusCode();
                var detailJson = await detailResponse.Content.ReadAsStringAsync();
                var detailData = JsonSerializer.Deserialize<TmdbMovieDetail>(detailJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // 2. Film kadrosu (yonetmen ve oyuncular icin)
                var creditsUrl = $"{_baseUrl}/movie/{movieId}/credits?api_key={_apiKey}&language=tr-TR";
                var creditsResponse = await _httpClient.GetAsync(creditsUrl);
                creditsResponse.EnsureSuccessStatusCode();
                var creditsJson = await creditsResponse.Content.ReadAsStringAsync();
                var creditsData = JsonSerializer.Deserialize<TmdbCredits>(creditsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var director = creditsData?.Crew?.FirstOrDefault(c => c.Job == "Director")?.Name;
                var cast = creditsData?.Cast?.Take(10).Select(c => c.Name).ToList() ?? new List<string>();
                var genres = detailData?.Genres?.Select(g => g.Name).ToList() ?? new List<string>();

                return new MovieDetails
                {
                    Director = director,
                    Cast = cast,
                    Genres = genres
                };
            }
            catch
            {
                return null;
            }
        }

        // TMDb response modelleri
        private class TmdbSearchResponse
        {
            [JsonPropertyName("results")]
            public List<TmdbMovieResult> Results { get; set; } = new();
        }

        private class TmdbMovieResult
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("original_title")]
            public string? OriginalTitle { get; set; }

            [JsonPropertyName("overview")]
            public string? Overview { get; set; }

            [JsonPropertyName("release_date")]
            public string? ReleaseDate { get; set; }

            [JsonPropertyName("poster_path")]
            public string? PosterPath { get; set; }
        }

        private class TmdbMovieDetail
        {
            [JsonPropertyName("genres")]
            public List<TmdbGenre>? Genres { get; set; }
        }

        private class TmdbGenre
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;
        }

        private class TmdbCredits
        {
            [JsonPropertyName("cast")]
            public List<TmdbCastMember>? Cast { get; set; }

            [JsonPropertyName("crew")]
            public List<TmdbCrewMember>? Crew { get; set; }
        }

        private class TmdbCastMember
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("character")]
            public string? Character { get; set; }
        }

        private class TmdbCrewMember
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("job")]
            public string Job { get; set; } = null!;
        }

        private class MovieDetails
        {
            public string? Director { get; set; }
            public List<string> Cast { get; set; } = new();
            public List<string> Genres { get; set; } = new();
        }
    }
}
