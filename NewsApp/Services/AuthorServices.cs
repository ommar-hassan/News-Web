using NewsApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace NewsApp.Services
{
    public class AuthorServices : IAuthorServices
    {
        private readonly Uri baseAddress = new("https://localhost:7285/api/");
        private readonly HttpClient _client;

        public AuthorServices()
        {
            _client = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }

        public async Task<string> DeleteAuthor(string id)
        {
            var response = _client.DeleteAsync($"{_client.BaseAddress}Authors/deleteauthor/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var author = JsonConvert.DeserializeObject<AuthorViewModel>(result);

                return author.Message;
            }

            return string.Empty;
        }

        public async Task<AuthorViewModel> GetAuthorById(string id)
        {
            var response = _client.GetAsync($"{_client.BaseAddress}Authors/getauthor/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var author = JsonConvert.DeserializeObject<AuthorViewModel>(result);

                return author;
            }

            return new AuthorViewModel();
        }

        public async Task<AuthorIndexViewModel> GetAuthors(string? search, string sortType, string sortOrder, int pageSize, int pageNumber)
        {
            var queryString = $"?search={search}&sortType={sortType}&sortOrder={sortOrder}&pageSize={pageSize}&pageNumber={pageNumber}";

            var response = await _client.GetAsync($"{_client.BaseAddress}Authors/getauthors{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var authors = JsonConvert.DeserializeObject<List<AuthorViewModel>>(result);

                var totalAuthors = await GetAuthorsCount(search);
                var totalPages = (int)Math.Ceiling(totalAuthors / (double)pageSize);

                var viewModel = new AuthorIndexViewModel
                {
                    Authors = authors,
                    Search = search,
                    SortType = sortType,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return viewModel;
            }

            return new AuthorIndexViewModel();
        }

        private async Task<int> GetAuthorsCount(string? search)
        {
            var response = _client.GetAsync($"{_client.BaseAddress}Authors/count?search={search}").Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var count = JsonConvert.DeserializeObject<int>(result);

                return count;
            }

            return -1;
        }

        public async Task<string> Login(LoginViewModel user)
        {
            var requestBody = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = _client.PostAsync($"{_client.BaseAddress}Authors/login", requestBody).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return errorResponse;
            }

            return string.Empty;
        }

        public async Task<string> Register(RegisterViewModel user)
        {
            var requestBody = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = _client.PostAsync($"{_client.BaseAddress}Authors/register", requestBody).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return errorResponse;
            }

            return string.Empty;
        }

        public async Task<string> UpdateAuthor(string id, UpdateAuthorViewModel updatedAuthor)
        {
            var requestBody = new StringContent(JsonConvert.SerializeObject(updatedAuthor), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{_client.BaseAddress}Authors/updateauthor/{id}", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var author = JsonConvert.DeserializeObject<AuthorViewModel>(result);

                return author.Message;
            }

            return string.Empty;
        }
    }
}