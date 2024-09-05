namespace staj_ecommerce_api.Models
{
    public class ShopRequest
    {
        public int ShopRequestId { get; set; }
        public string? ShopName { get; set; }
        public string? TaxNumber { get; set; }
        public string? CompanyPhoneNumber { get; set;}
        public string? ShopAddress { get; set;}
        public string? ApproveStatus { get; set;}
        public int UserId { get; set;}
    }
}
