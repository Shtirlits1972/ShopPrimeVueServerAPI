namespace ShopPrimeVueServerAPI.Models
{
    public class OrderDetail
    {
        public int id { get; set; } = 0;

        public int OrderId { get; set; } = 0;

        public int ProductId { get; set; } = 0;

        public decimal Qty { get; set; } = 0;

        public string ProductName { get; set; } = "";

        public decimal Price { get; set; } = 0;

        public decimal RowSum { get; set; } = 0;

        public override string ToString()
        {
            return $" OrderDetail (id = {id}, OrderId = {OrderId},  ProductId = {ProductId}, Qty = {Qty}, ProductName = {ProductName}, Price = {Price}, RowSum = {RowSum}, )";
        }
    }
}
