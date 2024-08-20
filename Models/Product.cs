namespace staj_ecommerce_api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public float? Rate { get; set; }
        public int? CountOfReviews { get; set; }
    }
}
