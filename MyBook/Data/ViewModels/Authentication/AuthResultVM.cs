namespace MyBook.Data.ViewModels.Authentication
{
    public class AuthResultVM
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
