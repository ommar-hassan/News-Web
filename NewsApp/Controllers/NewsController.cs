using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsApp.Models;
using NewsApp.Services;

namespace NewsApp.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsServices _newsServices;
        private readonly IAuthorServices _authorServices;

        public NewsController(INewsServices newsServices, IAuthorServices authorServices)
        {
            _newsServices = newsServices;
            _authorServices = authorServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? authorId, string? search, string sortType = "Title", string sortOrder = "asc", int pageSize = 10, int pageNumber = 1)
        {
            var authors = await _authorServices.GetAuthors("", "UserName", "asc", 100, 1);
            ViewBag.Authors = new SelectList(authors.Authors, "Id", "UserName", authorId);
            var news = await _newsServices.GetAllNewsAsync(search, sortType, sortOrder, pageSize, pageNumber);
            if (authorId != null)
            {
                ViewBag.AuthorId = authorId;
                var authorNews = news.News.Where(x => x.Author.Id == authorId);
                news.News = authorNews.ToList();
                news.TotalPages = (int)Math.Ceiling((double)authorNews.Count() / news.PageSize);
                return View(news);
            }            
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsServices.GetNewsByIdAsync(id);
            return View("Details", news);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var news = await _newsServices.GetNewsByIdAsync(id);
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var authors = await _authorServices.GetAuthors("", "UserName", "asc", 100, 1);
            ViewBag.Authors = new SelectList(authors.Authors, "Id", "UserName");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound($"News with ID {id} is not found");

            var news = await _newsServices.GetNewsByIdAsync(id);
            var authors = await _authorServices.GetAuthors("", "UserName", "asc", 100, 1);
            ViewBag.Authors = new SelectList(authors.Authors, "Id", "UserName", news.Author.Id);
            ViewBag.ImageUrl = news.ImageUrl;
            return View(new UpdateNewsViewModel 
            {
                AuthorId = news.Author.Id,
                Description = news.Description,
                Title = news.Title,
                PublicationDate = news.PublicationDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateNewsViewModel news)
        {
            if (id == 0)
                return NotFound($"News with ID {id} is not found");

            if (!IsPublicationDateValid(news.PublicationDate))
            {
                ModelState.AddModelError(string.Empty, "Publication date must be between today and one week from today.");
                return View(news);
            }
            var newsById = await _newsServices.GetNewsByIdAsync(id);
            var authors = await _authorServices.GetAuthors("", "UserName", "asc", 100, 1);
            ViewBag.Authors = new SelectList(authors.Authors, "Id", "UserName", news.AuthorId);
            ViewBag.ImageUrl = newsById.ImageUrl;

            if (ModelState.IsValid)
            {
                var result = await _newsServices.UpdateNewsAsync(id, news);

                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong.");
                    return View(news);
                }
            }

            return View("Edit", news);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNewsViewModel news)
        {
            var authors = await _authorServices.GetAuthors("", "UserName", "asc", 100, 1);
            ViewBag.Authors = new SelectList(authors.Authors, "Id", "UserName");

            if (!IsPublicationDateValid(news.PublicationDate))
            {
                ModelState.AddModelError(string.Empty, "Publication date must be between today and one week from today.");
                return View(news);
            }

            if (ModelState.IsValid)
            {
                var result = await _newsServices.CreateNewsAsync(news);

                if (result)
                {
                    return RedirectToAction("Index");
                }
                else {
                    ModelState.AddModelError(string.Empty, "Something went wrong.");
                    return View(news);
                }
            }

            return View(news);
        }

        private bool IsPublicationDateValid(DateTime publishedDate)
        {
            DateTime today = DateTime.Today;
            DateTime oneWeekFromToday = today.AddDays(7);

            return publishedDate >= today && publishedDate <= oneWeekFromToday;
        }

        [HttpPost]
        public IActionResult Delete(int id, NewsViewModel news)
        {
            if (id == 0)
                return NotFound();

            var result = _newsServices.DeleteNews(id);

            if (result)
                return RedirectToAction("Index");

            return View(news);
        }
    }
}
