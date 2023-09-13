using NewsApp.Models;

namespace NewsApp.Services
{
    public interface INewsServices
    {
        Task<NewsIndexViewModel> GetAllNewsAsync(string? search, string sortType, string sortOrder, int pageSize, int pageNumber);
        Task<NewsViewModel> GetNewsByIdAsync(int id);
        Task<bool> CreateNewsAsync(CreateNewsViewModel news);
        Task<bool> UpdateNewsAsync(int id, UpdateNewsViewModel news);
        bool DeleteNews(int id);
    }
}
