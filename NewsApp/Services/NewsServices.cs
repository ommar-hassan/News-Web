using NewsApp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace NewsApp.Services
{
    public class NewsServices : INewsServices
    {
        private readonly Uri baseAddress = new("https://localhost:7285/api/");
        private readonly HttpClient _client;

        public NewsServices()
        {
            _client = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }

        public async Task<bool> CreateNewsAsync(CreateNewsViewModel news)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(news.Title), "Title");
                content.Add(new StringContent(news.Description), "Description");
                content.Add(new StringContent(news.AuthorId.ToString()), "AuthorId");
                content.Add(new StringContent(news.PublicationDate.ToString("yyyy-MM-dd")), "PublicationDate");

                if (news.Image != null)
                {
                    // Add the image as a file
                    var fileContent = new StreamContent(news.Image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = news.Image.FileName
                    };
                    content.Add(fileContent);
                }

                var response = await _client.PostAsync($"{_client.BaseAddress}News", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating news: {ex.Message}");
            }
        }

        public bool DeleteNews(int id)
        {
            var response = _client.DeleteAsync($"{_client.BaseAddress}News/{id}").Result;

            return response.IsSuccessStatusCode;

        }

        public async Task<NewsIndexViewModel> GetAllNewsAsync(string? search, string sortType, string sortOrder, int pageSize, int pageNumber)
        {
            var queryString = $"?search={search}&sortType={sortType}&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}";

            var response = await _client.GetAsync($"{_client.BaseAddress}News{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<List<NewsViewModel>>(result);

                var totalNews = await GetNewsCount(search);
                var totalPages = (int)Math.Ceiling((double)totalNews / pageSize);

                return new NewsIndexViewModel 
                {
                    News = news,
                    Search = search,
                    SortType = sortType,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }

            return new NewsIndexViewModel();
        }

        public async Task<NewsViewModel> GetNewsByIdAsync(int id)
        {
            var response = _client.GetAsync($"{_client.BaseAddress}News/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsViewModel>(result);

                return news;
            }

            return new NewsViewModel();
        }

        public async Task<bool> UpdateNewsAsync(int id, UpdateNewsViewModel news)
        {
            try
            {
                using var content = new MultipartFormDataContent
                {
                    { new StringContent(news.Title), "Title" },
                    { new StringContent(news.Description), "Description" },
                    { new StringContent(news.AuthorId.ToString()), "AuthorId" },
                    { new StringContent(news.PublicationDate.ToString("yyyy-MM-dd")), "PublicationDate" }
                };

                if (news.Image != null)
                {
                    // Add the image as a file
                    var fileContent = new StreamContent(news.Image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = news.Image.FileName
                    };
                    content.Add(fileContent);
                }

                var response = await _client.PutAsync($"{_client.BaseAddress}News/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating news: {ex.Message}");
            }
        }

        private async Task<int> GetNewsCount(string? search)
        {
            var response = _client.GetAsync($"{_client.BaseAddress}News/count?search={search}").Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var count = JsonConvert.DeserializeObject<int>(result);

                return count;
            }

            return -1;
        }
    }
}
