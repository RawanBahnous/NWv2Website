namespace Mvc.Models
{
    public class IndexViewModel
    {
        public List<Author> Authors { get; set; }
        public List<News> News { get; set; }
        public News LastNews { get; set; }
    }
}
