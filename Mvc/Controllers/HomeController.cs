using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5217/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IActionResult> DetailsNews(int id)
        {
            var response = await _httpClient.GetAsync($"News/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var news = await response.Content.ReadFromJsonAsync<News>();

            if (news == null)
                return NotFound();

            var authorResponse = await _httpClient.GetAsync($"Authors/{news.AuthorID}");
            if (!authorResponse.IsSuccessStatusCode)
                return NotFound();

            var author = await authorResponse.Content.ReadFromJsonAsync<Author>();

            if (author == null)
                return NotFound();

            var viewModel = new IndexViewModel
            {
                News = new List<News> { news },
                Authors = new List<Author> { author }
            };

            return View("DetailsNews", news);
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage getAuthorsResponse = await _httpClient.GetAsync("/api/Authors");
            List<Author> allAuthors = new List<Author>();
            if (getAuthorsResponse.IsSuccessStatusCode)
            {
                string authorsContent = await getAuthorsResponse.Content.ReadAsStringAsync();
                allAuthors = JsonConvert.DeserializeObject<List<Author>>(authorsContent);
            }

            HttpResponseMessage getAllNewsResponse = await _httpClient.GetAsync("/api/News");
            List<News> allNews = new List<News>();
            if (getAllNewsResponse.IsSuccessStatusCode)
            {
                string newsContent = await getAllNewsResponse.Content.ReadAsStringAsync();
                allNews = JsonConvert.DeserializeObject<List<News>>(newsContent);
            }

            News lastNews = allNews.LastOrDefault();

            var viewModel = new IndexViewModel
            {
                Authors = allAuthors,
                News = allNews,
                LastNews = lastNews
            };

            return View(viewModel);
        }
        public async Task<IActionResult> SortByDate(string sortBy)
        {
            string apiUrl = "http://localhost:5217/api/News/";

            // Construct the API URL based on the selected sorting option
            string apiEndpoint = string.Empty;
            switch (sortBy)
            {
                case "newsasc":
                    apiEndpoint = "newsasc";
                    break;
                case "newsdesc":
                    apiEndpoint = "newsdesc";
                    break;
                default:
                    apiEndpoint = "default";
                    break;
            }

            HttpResponseMessage getNewsResponse = await _httpClient.GetAsync(apiUrl + apiEndpoint);
            List<News> sortedNews = new List<News>();
            if (getNewsResponse.IsSuccessStatusCode)
            {
                string newsContent = await getNewsResponse.Content.ReadAsStringAsync();
                sortedNews = JsonConvert.DeserializeObject<List<News>>(newsContent);
            }

            // Fetch all authors from the API
            HttpResponseMessage getAuthorsResponse = await _httpClient.GetAsync("/api/Authors");
            List<Author> allAuthors = new List<Author>();
            if (getAuthorsResponse.IsSuccessStatusCode)
            {
                string authorsContent = await getAuthorsResponse.Content.ReadAsStringAsync();
                allAuthors = JsonConvert.DeserializeObject<List<Author>>(authorsContent);
            }

            var viewModel = new IndexViewModel
            {
                Authors = allAuthors,
                News = sortedNews
            };

            return View("SortByDate", viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}