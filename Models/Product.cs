using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopPrimeVueServerAPI.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; } = 0;
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public string CategoryName { get; set; } = "";
        public string Foto { get; set; } = "X";

        public override string ToString()
        {
            return $" Product (id = {id}, ProductName = {ProductName}, Price = {Price}, CategoryId = {CategoryId},  CategoryName = {CategoryName}, Foto = {Foto} )";
        }
    }
}
