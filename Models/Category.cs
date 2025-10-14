using System.ComponentModel.DataAnnotations;

namespace ShopPrimeVueServerAPI.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; } = 0;
        public string categoryName { get; set; } = "";
        public override string ToString()
        {
            return $" Category (id = {id}, categoryName = {categoryName} )";
        }
    }
}
