using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public interface IGoogleBooksService
    {
        Task<List<SearchResultItemDto>> SearchBooksAsync(string query);
    }
}
