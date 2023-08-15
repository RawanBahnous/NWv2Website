using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mvc.Controllers
{
    
    public class AdminController : Controller
    {
        private const string BaseUrl = "http://localhost:5217";

        private readonly HttpClient _httpClient;

        public AdminController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/register", user);

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful, redirect or handle accordingly
                    return RedirectToAction("Login", "Admin");
                }
                else
                {
                    // Registration failed, handle error response
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", errorMessage);
                    return View(user);
                }
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", errorMessage);
                    return View(loginData);
                }
            }
        }

        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleModel role)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/addrole", role);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", errorMessage);
                    return View(role);
                }
            }
        }

       
    }
}
