using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using System.Net.Http.Headers;

namespace Mvc.Controllers
{
    public class AuthorController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthorController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5217/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> GetAllAuthors()
        {
            var response = await _httpClient.GetAsync("Authors");
            response.EnsureSuccessStatusCode();
            var AuthorsList = await response.Content.ReadFromJsonAsync<List<Author>>();

            var AuthorsData = AuthorsList.Select(Authors => new Author
            {
                Id = Authors.Id, 
                Name = Authors.Name,
                
            }).ToList();

            return View(AuthorsData);
        }

        [HttpGet]
        [Route("Authors/SearchAuthors")]
        public async Task<IActionResult> SearchAuthors(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5217/api/Authors/search?searchName={searchTerm}");
            response.EnsureSuccessStatusCode();
            var authors = await response.Content.ReadFromJsonAsync<List<Author>>();
            return View("SearchAuthors", authors);
        }
        [HttpGet]
        [Route("Authors/GetAuthorsAscending/{sortOrder}")]
        public async Task<IActionResult> GetAuthorsAscending(string sortOrder)
        {
            var response = await _httpClient.GetAsync($"api/Authors/authorasc?sortOrder={sortOrder}");
            response.EnsureSuccessStatusCode();
            var authors = await response.Content.ReadFromJsonAsync<List<Author>>();
            return View("GetAuthorsAscending", authors);
        }



        public async Task<IActionResult> DetailsAuthors(int id)
        {
            var response = await _httpClient.GetAsync($"Authors/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var Authors = await response.Content.ReadFromJsonAsync<Author>();
            return View(Authors);
        }
        //create new Author
        public IActionResult CreateNewAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewAuthor(Author newAuthor)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"Authors/", newAuthor);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllAuthors", "Author");
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMessage);
                return View(newAuthor);
            }
        }

        
    

        // Delete an author
        
        public IActionResult DeleteAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var response = await _httpClient.DeleteAsync($"Authors/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            return RedirectToAction("GetAllAuthors");
        }



    }
}
