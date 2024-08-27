namespace staj_ecommerce_api.Security
{
    public class MyToken
    {
        public string AccesToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
