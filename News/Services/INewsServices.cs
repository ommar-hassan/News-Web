using NewsAPI.Dto;
using NewsAPI.Models;

namespace NewsAPI.Services
{
    public interface INewsServices
    {
        Task<List<NewsDto>> GetAllNews(string? search, string sortType, string sortOrder, int pageSize, int pageNumber);
        Task<NewsDto> GetNewsById(int id);
        Task<int> GetNewsCount(string? search);
        Task<NewsDto> CreateNews(CreateNewsDto dto);
        Task<NewsDto> UpdateNews(int id, UpdateNewsDto dto);
        Task<NewsDto> DeleteNews(int id);
        Task<bool> IsValidNews(int id);
    }
}
