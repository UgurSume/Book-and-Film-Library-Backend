using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public interface ITmdbService
    {
        Task<List<SearchResultItemDto>> SearchMoviesAsync(string query);
    }
}