using System.ComponentModel.DataAnnotations;

namespace ShopPrimeVueServerAPI.Models
{
    public class OrderHead
    {
        [Key]
        public int id { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string UsersName { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public DateTime OrderData { get; set; } = new DateTime(2000, 1, 1);
        public decimal TotalPrice { get; set; } = 0;

        public override string ToString()
        {
            return $" OrderHead (id = {id}, UserId = {UserId}, UsersName = {UsersName}, OrderNumber = {OrderNumber}, OrderData = {OrderData}, TotalPrice = {TotalPrice})";
        }
    }
}
