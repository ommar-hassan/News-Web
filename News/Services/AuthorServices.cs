using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsAPI.Dto;
using NewsAPI.Helpers;
using NewsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace NewsAPI.Services
{
    public class AuthorServices : IAuthorServices
    {
        private readonly UserManager<Author> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthorServices(UserManager<Author> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<string> AddRole(AddRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null || !await _roleManager.RoleExistsAsync(dto.RoleName))
                return "Invalid User ID or Role Name";

            if (await _userManager.IsInRoleAsync(user, dto.RoleName))
                return "User already has this role";

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);

            return result.Succeeded ? string.Empty : "Something went wrong";

        }
        public async Task<bool> IsValidAuthor(string id)
        {
            return await _userManager.FindByIdAsync(id) != null;
        }
        public async Task<AuthorDto> GetAuthorById(string id)
        {
            if (await IsValidAuthor(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                var token = await CreateJwtToken(user);

                var roles = await _userManager.GetRolesAsync(user);
                return new AuthorDto
                {
                    Id = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList(),
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresOn = token.ValidTo
                };
            }
            else
            {
                return new AuthorDto { Message = "Invalid Author ID" };
            }
        }

        public async Task<List<AuthorDto>> GetAuthors(string? search, string sortType, string sortOrder, int pageSize, int pageNumber)
        {
            var authorsQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                authorsQuery = authorsQuery.Where(x => x.FirstName.Contains(search)
                || x.LastName.Contains(search)
                || x.Email.Contains(search) 
                || x.UserName.Contains(search));
            }

            authorsQuery = sortOrder == "desc"
                ? authorsQuery.OrderByDescending(GetSortProperty(sortType))
                : (IQueryable<Author>)authorsQuery.OrderBy(GetSortProperty(sortType));

            if (pageSize < 1)
                pageSize = 1;
            if (pageSize > 20)
                pageSize = 20;
            if (pageNumber < 1)
                pageNumber = 1;

            authorsQuery = authorsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var usersDto = new List<AuthorDto>();

            foreach (var author in authorsQuery)
            {
                var roles = await _userManager.GetRolesAsync(author);
                var token = await CreateJwtToken(author);

                usersDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Email = author.Email,
                    UserName = author.UserName,
                    Roles = roles.ToList(),
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresOn = token.ValidTo
                });
            }

            return usersDto;
        }

        private static Expression<Func<Author, object>> GetSortProperty(string sortType)
            => sortType switch
            {
                "FirstName" => x => x.FirstName,
                "LastName" => x => x.LastName,
                "Email" => x => x.Email,
                "UserName" => x => x.UserName,
                _ => x => x.Id
            };

        public async Task<AuthorDto> GetToken(TokenRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthorDto { Message = "Email or password is incorrect" };
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthorDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Message = "Token generated successfully",
                Roles = roles.ToList()
            };
        }

        public async Task<AuthorDto> Resgister(RegisterDto user)
        {
            if (await _userManager.FindByNameAsync(user.UserName) != null)
            {
                return new AuthorDto { Message = "User already exists" };
            }
            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                return new AuthorDto { Message = "Email already exists" };
            }

            var newUser = new Author
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthorDto { Message = errors };
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            var jwtSecurityToken = await CreateJwtToken(newUser);

            return new AuthorDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Message = "User created successfully",
                Roles = new List<string> { "User" }
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(Author user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.ExpiryInDays).ToLocalTime(),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<AuthorDto> UpdateAuthor(string id, UpdateAuthorDto dto)
        {
            if (await IsValidAuthor(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;

                if (await _userManager.FindByNameAsync(dto.UserName) != null && user.UserName != dto.UserName)
                {
                    return new AuthorDto { Message = "User already exists" };
                }

                user.UserName = dto.UserName;
                if (await _userManager.FindByEmailAsync(dto.Email) != null && dto.Email != user.Email)
                {
                    return new AuthorDto { Message = "Email already exists" };
                }
                user.Email = dto.Email;

                await _userManager.UpdateAsync(user);

                return new AuthorDto
                {
                    Id = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsAuthenticated = true,
                };
            }
            else
            {
                return new AuthorDto { Message = "Invalid Author ID" };
            }
        }

        public async Task<AuthorDto> DeleteAuthor(string id)
        {
            if (await IsValidAuthor(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);

                return new AuthorDto
                {
                    Id = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsAuthenticated = true,
                };
            }
            else
            {
                return new AuthorDto { Message = "Invalid Author ID" };
            }
        }

        public async Task<int> GetAuthorCount(string? search)
        {
            var authorsQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                authorsQuery = authorsQuery.Where(x => x.FirstName.Contains(search)
                || x.LastName.Contains(search)
                || x.Email.Contains(search)
                || x.UserName.Contains(search));
            }

            return await authorsQuery.CountAsync();
        }
    }
}