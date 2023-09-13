using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Dto;
using NewsAPI.Services;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorServices _authServices;

        public AuthorsController(IAuthorServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto user)
        {
            var result = await _authServices.Resgister(user);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Token([FromBody] TokenRequestDto model)
        {
            var result = await _authServices.GetToken(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto model)
        {
            var result = await _authServices.AddRole(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);

        }

        [HttpGet("getauthors")]
        public async Task<IActionResult> GetAuthors(string? search, string sortType, string sortOrder, int pageSize = 5, int pageNumber = 1)
        {
            var result = await _authServices.GetAuthors(search, sortType, sortOrder, pageSize, pageNumber);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetAuthorCount(string? search)
        {
            try
            {
                var count = await _authServices.GetAuthorCount(search);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("getauthor/{id}")]
        public async Task<IActionResult> GetAuthor(string id)
        {
            if (!await _authServices.IsValidAuthor(id))
                return BadRequest("Invalid Author ID");

            var result = await _authServices.GetAuthorById(id);

            return Ok(result);
        }

        [HttpPut("updateauthor/{id}")]
        public async Task<IActionResult> UpdateAuthor(string id, [FromBody] UpdateAuthorDto dto)
        {
            if (!await _authServices.IsValidAuthor(id))
                return BadRequest("Invalid Author ID");

            var result = await _authServices.UpdateAuthor(id, dto);

            return Ok(result);
        }

        [HttpDelete("deleteauthor/{id}")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {

            if (!await _authServices.IsValidAuthor(id))
                return BadRequest("Invalid Author ID");

            var result = await _authServices.DeleteAuthor(id);

            return Ok(result);
        }
    }
}
