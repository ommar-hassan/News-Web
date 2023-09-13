using NewsAPI.Dto;

namespace NewsAPI.Services
{
    public interface IAuthorServices
    {
        Task<AuthorDto> Resgister(RegisterDto user);
        Task<AuthorDto> GetToken(TokenRequestDto dto);
        Task<string> AddRole(AddRoleDto dto);
        Task<List<AuthorDto>> GetAuthors(string? search, string sortType, string sortOrder, int pageSize, int pageNumber);
        Task<AuthorDto> GetAuthorById(string id);
        Task<int> GetAuthorCount(string? search);
        Task<bool> IsValidAuthor(string id);
        Task<AuthorDto> UpdateAuthor(string id, UpdateAuthorDto dto);
        Task<AuthorDto> DeleteAuthor(string id);
    }
}
