using System.ComponentModel.DataAnnotations;

namespace ShopPrimeVueServerAPI.Models
{
    public class Users
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(250)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Password { get; set; } = string.Empty;

        [StringLength(250)]
        public string Role { get; set; } = "user";

        [StringLength(250)]
        public string UsersName { get; set; } = string.Empty;

        [Required]
        public bool isAppruved { get; set; } = true;

        public override string ToString()
        {
            return $"Users(id = {id}, Email = {Email}, Role = {Role}, UsersName = {UsersName}, isAppruved = {isAppruved})";
        }
    }
}
