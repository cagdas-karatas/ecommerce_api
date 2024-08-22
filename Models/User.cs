using System.ComponentModel.DataAnnotations;

namespace staj_ecommerce_api.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kullanıcı ismi boş bırakılamaz")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Şifre boş bırakılamaz")]
        public string Password { get; set; }
        [Required(ErrorMessage = "İsim boş bırakılamaz")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İsim en az 2, en çok 50 karakter olmalı")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Soy isim boş bırakılamaz")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soy isim en az 2, en çok 50 karakter olmalı")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Telefon boş bırakılamaz")]
        [Phone(ErrorMessage ="Lütfen geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir email giriniz")]
        public string Email { get; set; }
        public string UserType { get; set; }
    }
}
