namespace NewsAPIProject.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public List<string> Roles { get; set; }

        public string Token { get; set; }

        public DateTime Expire { get; set; }

    }
}
