namespace Mvc.Models
{
    public class UploadImageModel
    {
        public int NewsId { get; set; }
        public IFormFile ImageFile { get; set; }
    }

}
