using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.Dto;
using NewsAPI.Models;
using System.Linq.Expressions;

namespace NewsAPI.Services
{
    public class NewsServices : INewsServices
    {
        private readonly IImageServices _imageServices;
        private readonly ApplicationDbContext _context;

        public NewsServices(IImageServices imageServices, ApplicationDbContext context)
        {
            _imageServices = imageServices;
            _context = context;
        }

        public async Task<NewsDto> CreateNews(CreateNewsDto dto)
        {

            if (!ValidatePublicationDate(dto.PublicationDate))
                throw new Exception("Publication Date must be between today and a week from today.");


            var imageUrl = await _imageServices.UploadImage(dto.Image);

            var news = new News
            {
                AuthorId = dto.AuthorId,
                Title = dto.Title,
                Description = dto.Description,
                Image = imageUrl,
                PublicationDate = dto.PublicationDate
            };

            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();

            var result = new NewsDto
            {
                Id = news.Id,
                Title = news.Title,
                Description = news.Description,
                ImageUrl = news.Image,
                PublicationDate = news.PublicationDate,
                CreationDate = news.CreationDate,
                Author = new AuthorDto
                {
                    Id = news.Author.Id,
                    FirstName = news.Author.FirstName,
                    LastName = news.Author.LastName,
                    UserName = news.Author.UserName,
                    Email = news.Author.Email
                }
            };

            return result;
        }

        private bool ValidatePublicationDate(DateTime publicationDate)
        {
            DateTime today = DateTime.Today.ToLocalTime();
            DateTime nextWeek = today.AddDays(7).ToLocalTime();

            return publicationDate >= today && publicationDate <= nextWeek;
        }

        public async Task<NewsDto> DeleteNews(int id)
        {
            try
            {
                if (!await IsValidNews(id))
                    throw new Exception("Invalid ID");
                var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);

                var result = await GetNewsById(id);

                _context.News.Remove(news);
                await _context.SaveChangesAsync();


                return result;
            }
            catch (Exception e)
            {

                throw new Exception($"Something went wrong. {e}");
            }
            
        }

        public async Task<List<NewsDto>> GetAllNews(string? search, string sortType, string sortOrder, int pageSize, int pageNumber)
        {
            var newsQuery = _context.News.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                newsQuery = newsQuery.Where(n => n.Title.Contains(search)
                || n.Description.Contains(search)
                || n.AuthorId.Equals(search));
            }

            newsQuery = sortOrder == "desc" 
                ? newsQuery.OrderByDescending(GetSortProperty(sortType)) 
                : (IQueryable<News>)newsQuery.OrderBy(GetSortProperty(sortType));

            if (pageSize < 1)
                pageSize = 1;
            if (pageSize > 20)
                pageSize = 20;
            if (pageNumber < 1)
                pageNumber = 1;

            newsQuery = newsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var news = await newsQuery
                .Include(a => a.Author)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    ImageUrl = n.Image,
                    Author = new AuthorDto
                    {
                        Id = n.Author.Id,
                        FirstName = n.Author.FirstName,
                        LastName = n.Author.LastName,
                        UserName = n.Author.UserName,
                        Email = n.Author.Email,
                        // You can populate other properties of AuthorDto as needed
                    },
                    PublicationDate = n.PublicationDate,
                    CreationDate = n.CreationDate
                })
                .ToListAsync();

            return news;
        }

        private static Expression<Func<News, object>> GetSortProperty(string sortType)
            => sortType switch
        {
            "Title" => n => n.Title,
            "Description" => n => n.Description,
            "PublicationDate" => n => n.PublicationDate,
            _ => n => n.Id
        };

        public async Task<NewsDto> GetNewsById(int id)
        {
            if (!await IsValidNews(id))
                throw new Exception("Invalid ID");

            var news = await _context.News
                .Include(x => x.Author)
                .Select(x => new NewsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ImageUrl = x.Image,
                    PublicationDate = x.PublicationDate,
                    CreationDate = x.CreationDate,
                    Author = new AuthorDto
                    {
                        Id = x.Author.Id,
                        FirstName = x.Author.FirstName,
                        LastName = x.Author.LastName,
                        UserName = x.Author.UserName,
                        Email = x.Author.Email
                    }
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            return news;
        }

        public async Task<bool> IsValidNews(int id)
        {
            return await _context.News.AnyAsync(x => x.Id == id);
        }

        public async Task<NewsDto> UpdateNews(int id, UpdateNewsDto dto)
        {
            if (!await IsValidNews(id))
                throw new Exception("Invalid ID");

            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);

            news.Title = dto.Title;
            news.Description = dto.Description;
            news.PublicationDate = dto.PublicationDate;
            news.AuthorId= dto.AuthorId;
            if (dto.Image != null)
                news.Image = await _imageServices.UploadImage(dto.Image);
            _context.News.Update(news);
            await _context.SaveChangesAsync();

            var result = new NewsDto
            {
                Id = news.Id,
                Title = news.Title,
                Description = news.Description,
                ImageUrl = news.Image,
                PublicationDate = news.PublicationDate,
                CreationDate = news.CreationDate,
                Author = new AuthorDto
                {
                    Id = news.Author.Id,
                    FirstName = news.Author.FirstName,
                    LastName = news.Author.LastName,
                    UserName = news.Author.UserName,
                    Email = news.Author.Email
                },
            };
            return result;
        }

        public async Task<int> GetNewsCount(string? search)
        {
            var newsQuery = _context.News.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                newsQuery = newsQuery.Where(n => n.Title.Contains(search)
                                                                || n.Description.Contains(search)
                                                                || n.AuthorId.Equals(search));
            }

            return await newsQuery.CountAsync();
        }
    }
}
