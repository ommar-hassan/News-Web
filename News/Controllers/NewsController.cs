using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Dto;
using NewsAPI.Services;


namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IAuthorServices _authorServices;
        private readonly INewsServices _newsService;

        public NewsController(INewsServices newsService, IAuthorServices authorServices)
        {
            _newsService = newsService;
            _authorServices = authorServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNews([FromForm] CreateNewsDto dto)
        {
            if (!await _authorServices.IsValidAuthor(dto.AuthorId))
                return BadRequest("Invalid Author Id");

            var result = await _newsService.CreateNews(dto);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNews(string? search,
            string sortType,
            string sortOrder, 
            int pageSize = 5,
            int pageNumber = 1)
        {
            var result = await _newsService.GetAllNews(search, sortType, sortOrder, pageSize, pageNumber);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var result = await _newsService.GetNewsById(id);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetNewsCount(string? search)
        {
            try
            {
                var count = await _newsService.GetNewsCount(search);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromForm] UpdateNewsDto dto)
        {
            if (!await _newsService.IsValidNews(id))
                return BadRequest("Invalid News Id");
            if (!await _authorServices.IsValidAuthor(dto.AuthorId))
                return BadRequest("Invalid Author Id");


            var result = await _newsService.UpdateNews(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            if (!await _newsService.IsValidNews(id))
                return BadRequest("Invalid News Id");

            var result = await _newsService.DeleteNews(id);

            return Ok(result);
        }
    }
}
