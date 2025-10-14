namespace ShopPrimeVueServerAPI.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UsersName { get; set; } = string.Empty;
        public bool isAppruved { get; set; }
    }
}
