using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPIProject.Data;
using NewsAPIProject.Models;
using System.Data;

namespace WebApplication1.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class AuthorsController : ControllerBase
        {

            public NewsDbContext NewsService { get; set; }
            public AuthorsController(NewsDbContext newsService)
            {
                NewsService = newsService;
            }

            [HttpGet]
            public ActionResult<List<Author>> GetAllAuthors()
            {
                return Ok(NewsService.Authors.ToList());
            }


            [HttpGet("authorasc")]
            public ActionResult<List<Author>> GetAuthorsAsced()
            {
                var Authorasc = NewsService.Authors.OrderBy(a => a.Name).ToList();
                if (Authorasc == null)
                {
                    return BadRequest("Can't Find Any Data");
                }
                return Ok(Authorasc);
            }

            [HttpGet("search")]
            public ActionResult<List<Author>> GetAuthorsByName(string searchName)
            {
                if (string.IsNullOrEmpty(searchName))
                {
                    return BadRequest("Search name is required.");
                }

                var authors = NewsService.Authors.Where(a => a.Name.Contains(searchName)).ToList();
                if (authors.Count == 0)
                {
                    return NotFound("No authors found matching the search criteria.");
                }

                return Ok(authors);
            }

            [HttpGet("{id}")]
            public ActionResult<Author> GetAuthorById(int id)
            {
                var Authors = NewsService.Authors.FirstOrDefault(a => a.Id == id);
                if (Authors == null)
                {
                    return NotFound();
                }
                return Ok(Authors);
            }


            //[Authorize(Roles = "admin")]
            [HttpDelete("{id}")]
            public ActionResult DeleteNewsById(int id)
            {
                var Authors = NewsService.Authors.FirstOrDefault(a => a.Id == id);
                if (Authors == null)
                {
                    return NotFound();
                }
                NewsService.Authors.Remove(Authors);
                NewsService.SaveChanges();
                return Ok(Authors);
            }

            //[Authorize(Roles = "admin")]
            [HttpPost]
            public ActionResult<Author> CreateAuthor([FromBody] Author author)
            {
                NewsService.Authors.Add(author);
                NewsService.SaveChanges();
                return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
            }


            //[Authorize(Roles = "admin")]
            [HttpPut("{id}")]
            public ActionResult<Author> UpdateAuthor(int id, [FromBody] Author updatedAuthor)
            {
                var author = NewsService.Authors.FirstOrDefault(a => a.Id == id);
                if (author == null)
                {
                    return NotFound();
                }

                author.Name = updatedAuthor.Name;
                NewsService.SaveChanges();
                return Ok(author);
            }




    }
}


