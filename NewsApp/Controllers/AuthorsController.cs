using Microsoft.AspNetCore.Mvc;
using NewsApp.Models;
using NewsApp.Services;
using Newtonsoft.Json;
using System.Reflection;

namespace NewsApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorServices _authorServices;

        public AuthorsController(IAuthorServices authServices)
        {
            _authorServices = authServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string sortType = "UserName", string sortOrder = "asc", int pageSize = 10, int pageNumber = 1)
        {
            var authors = await _authorServices.GetAuthors(search, sortType, sortOrder, pageSize, pageNumber);

            return View(authors);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                var errorMessage = await _authorServices.Register(user);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(user);
                }
            }

            return View("Register", user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var errorMessage = await _authorServices.Login(user);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(user);
                }
            }

            return View("Login", user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthor(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Author Id");

            var author = await _authorServices.GetAuthorById(id);

            return View("Details", author);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Author Id");

            var author = await _authorServices.GetAuthorById(id);

            return View(new UpdateAuthorViewModel
            {
                FirstName = author.FirstName,
                LastName = author.LastName,
                UserName = author.UserName,
                Email = author.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UpdateAuthorViewModel updatedAuthor)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Author Id");

            if (ModelState.IsValid)
            {
                var errorMessage = await _authorServices.UpdateAuthor(id, updatedAuthor);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("Index");
                }
                else {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(updatedAuthor);
                }
            }

            return View("Edit", updatedAuthor);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Author Id");

            var author = await _authorServices.GetAuthorById(id);

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, AuthorViewModel author)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid Author Id");

            var errorMessage = await _authorServices.DeleteAuthor(id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(author);
            }
        }
    }
}
