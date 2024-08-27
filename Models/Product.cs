using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace staj_ecommerce_api.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Mağaza kimliği alınamadı")]
        public int ShopId { get; set; }

        [Required(ErrorMessage = "Ürün adı boş bırakılamaz")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Ürün ismi en az 10, en çok 100 karakter olmalı")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Ürün açıklaması boş bırakılamaz")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Kategori tanımlanmadı")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Ürün fiyatı boş bırakılamaz")]
        public float Price { get; set; }

        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
