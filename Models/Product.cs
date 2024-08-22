using System.ComponentModel.DataAnnotations;

namespace staj_ecommerce_api.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ürün adı boş bırakılamaz")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Ürün ismi en az 10, en çok 10 karakter olmalı")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Satıcı ismi boş bırakılamaz")]
        public string SellerName { get; set; }
        [Required(ErrorMessage = "Ürün açıklaması boş bırakılamaz")]
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        [Required(ErrorMessage = "Ürün fiyatı boş bırakılamaz")]
        public float Price { get; set; }
        [Required(ErrorMessage = "Ürün görseli boş bırakılamaz")]
        public string ImageUrl { get; set; }
        public float? Rate { get; set; }
        public int? CountOfReviews { get; set; }
    }
}
