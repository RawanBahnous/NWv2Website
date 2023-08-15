using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net;

namespace Mvc.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _httpClient;

        public NewsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5217/api/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> GetAllNews()
        {
            var response = await _httpClient.GetAsync("News");
            response.EnsureSuccessStatusCode();
            var newsList = await response.Content.ReadFromJsonAsync<List<News>>();

            var newsData = newsList.Select(news => new News
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                PublicationDate = news.PublicationDate,
                CreationDate = news.CreationDate,
                Image = news.Image,
                AuthorName = news.AuthorName
            }).ToList();

            return View(newsData);
        }

        public async Task<IActionResult> SearchNews(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"News/search?searchTitle={searchTerm}");
            response.EnsureSuccessStatusCode();
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            return View(news);
        }

        public async Task<IActionResult> GetNewsAscending()
        {
            var response = await _httpClient.GetAsync("News/newsasc");
            response.EnsureSuccessStatusCode();
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            return View(news);
        }

        public async Task<IActionResult> GetNewsDescending()
        {
            var response = await _httpClient.GetAsync("News/newsdesc");
            response.EnsureSuccessStatusCode();
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            return View(news);
        }

        public async Task<IActionResult> DetailsNews(int id)
        {
            var response = await _httpClient.GetAsync($"News/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var news = await response.Content.ReadFromJsonAsync<News>();

            // Append the base URL to the image path
            if (!string.IsNullOrEmpty(news.Image))
            {
                news.Image = Url.Content("~/images/" + news.Image);
            }

            return View(news);
        }
        public IActionResult DeleteNews()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var response = await _httpClient.DeleteAsync($"News/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            return RedirectToAction("GetAllNews");
        }


        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(UploadImageModel model)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:5217/");

                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.NewsId.ToString()), "newsId");
                formData.Add(new StreamContent(model.ImageFile.OpenReadStream()), "imageFile", model.ImageFile.FileName);

                var response = await httpClient.PostAsync("api/News/upload-image", formData);

                if (response.IsSuccessStatusCode)
                {
                    var imagePath = await response.Content.ReadAsStringAsync();
                    ViewBag.ImagePath = imagePath;

                    // Redirect to GetAllNews action
                    return RedirectToAction("GetAllNews", "News");
                }
            }

            return RedirectToAction("GetAllNews", "News");

        }



        // Create News

        public IActionResult CreateNews()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateNews(News news)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/News/insert", news);

            if (response.IsSuccessStatusCode)
            {
                var createdNews = await response.Content.ReadFromJsonAsync<News>();
                return RedirectToAction("UploadImage", new { newsId = createdNews.Id });
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMessage);
                return View(news);
            }
        }

        //[Authorize(Roles = "admin")]
        //[HttpPost]
        //public async Task<IActionResult> CreateNews(News news)
        //{
        //    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/News/insert", news);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("GetAllNews", "News");
        //    }
        //    else
        //    {
        //        string errorMessage = await response.Content.ReadAsStringAsync();
        //        ModelState.AddModelError("", errorMessage);
        //        return View(news);
        //    }
        //}


        public async Task<IActionResult> GetNewsByAuthorId(int authorId)
        {
            var response = await _httpClient.GetAsync($"News/newsby/{authorId}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("No news found for the specified author ID.");
            }
            var newsList = await response.Content.ReadFromJsonAsync<List<News>>();
            return View(newsList);


        }
    }
}
