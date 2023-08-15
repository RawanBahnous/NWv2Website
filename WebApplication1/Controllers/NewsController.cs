using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPIProject.Data;
using NewsAPIProject.Models;
using System.Data;
using System.Globalization;
using System.Net.Http;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {


        //public NewsService NewsService { get; set; }
        //public NewsController(NewsService newsService ) { 
        //     NewsService = newsService;
        //}

        public NewsDbContext NewsService { get; set; }
        public NewsController(NewsDbContext newsService)
        {
            NewsService = newsService;
        }

        [HttpGet]
        public ActionResult<List<News>> GetAllNews()
        {
            //return NewsService.GetAllNews();
            return Ok(NewsService.News.ToList());
        }

        [HttpGet("search")]
        public ActionResult<List<Author>> filterNews(string searchTitle)
        {
            if (string.IsNullOrEmpty(searchTitle))
            {
                return BadRequest("Search name is required.");
            }

            var News = NewsService.News.Where(n => n.Title.Contains(searchTitle)).ToList();
            if (News.Count == 0)
            {
                return NotFound("No authors found matching the search criteria.");
            }

            return Ok(News);
        }


        [HttpGet("newsasc")]
        public ActionResult<List<News>> GetNewsAsced()
        {
            var newsasc = NewsService.News.OrderBy(n => n.PublicationDate).ToList();
            if (newsasc == null)
            {
                return BadRequest("Can't Find Any Data");
            }
            return Ok(newsasc);
        }



        [HttpGet("newsdesc")]
        public ActionResult<List<News>> GetAllByDateDesc()
        {
            var newstToOld = NewsService.News.OrderByDescending(n => n.PublicationDate).ToList();
            if (newstToOld == null)
            {
                return BadRequest("Can't Find Any Data");
            }
            return Ok(newstToOld);
        }


        [HttpGet("{id}")]
        public ActionResult<News> GetNewsById(int id)
        {
            var news = NewsService.News.FirstOrDefault(n => n.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            return Ok(news);
        }

        //[Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteNewsById(int id)
        {
            var news = NewsService.News.FirstOrDefault(n => n.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            NewsService.News.Remove(news);
            NewsService.SaveChanges();
            return Ok(news);
        }



        [HttpPost("insert")]
        public IActionResult InsertNews(News formData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var author = NewsService.Authors.FirstOrDefault(a => a.Id == formData.AuthorID);
            if (author == null)
                return BadRequest("Invalid author ID.");

            DateTime today = DateTime.Today;
            DateTime publicationDate = formData.PublicationDate.Date;
            DateTime maxPublicationDate = today.AddDays(7);
            if (publicationDate < today || publicationDate > maxPublicationDate)
            {
                ModelState.AddModelError("Publication Date", "Publication date must be between today and a week from today.");
                return BadRequest(ModelState);
            }
            var news = new News
            {
                Title = formData.Title,
                AuthorID = formData.AuthorID,
                Content = formData.Content,
                PublicationDate = formData.PublicationDate,
                CreationDate = DateTime.Now,
                Image = formData.Image
            };

            NewsService.News.Add(news);
            NewsService.SaveChanges();

            return Ok(news);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(int newsId, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                var fileName = $"{newsId}_{imageFile.FileName}";
                var filePath = Path.Combine("wwwroot", "images", fileName);

                await System.IO.File.WriteAllBytesAsync(filePath, imageData);
                var news = NewsService.News.FirstOrDefault(n => n.Id == newsId);
                if (news != null)
                {
                    news.Image = filePath; 
                    NewsService.SaveChanges(); 
                }

                return Ok();
            }

            return BadRequest("Image upload failed.");
        }


        //get news by authorid

        [HttpGet("newsby/{authorId}")]
        public IActionResult GetNewsByAuthorId(int authorId)
        {
            var newsList = NewsService.News.Where(n => n.AuthorID == authorId).ToList();
            if (newsList.Count == 0)
            {
                return NotFound("No news found for the specified author ID.");
            }

            return Ok(newsList);
        }



    }
}