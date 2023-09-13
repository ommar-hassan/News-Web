using NewsApp.Models;

namespace NewsApp.Services
{
    public interface IAuthorServices
    {
        Task<string> Register(RegisterViewModel user);
        Task<string> Login(LoginViewModel user);
        Task<AuthorIndexViewModel> GetAuthors(string? search, string sortType, string sortOrder, int pageSize, int pageNumber);
        Task<AuthorViewModel> GetAuthorById(string id);
        Task<string> UpdateAuthor(string id, UpdateAuthorViewModel updatedAuthor);
        Task<string> DeleteAuthor(string id);
    }
}
