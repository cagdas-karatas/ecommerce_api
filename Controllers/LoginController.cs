using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using staj_ecommerce_api.Models;
using staj_ecommerce_api.Security;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        public LoginController(IConfiguration _configuration)
        {
            this.connectionString = _configuration.GetConnectionString("mysql_connection_string");
            this.configuration = _configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LoginUser(UserDTO _user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "UserLogin";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("@u_name", _user.userName));
                command.Parameters.Add(new MySqlParameter("@u_password", HashPassword(_user.password)));

                connection.Open();

                User user = null;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        user = new User();
                        user.UserId = reader.GetInt32("user_id");
                        user.UserName = reader.GetString("user_name");
                        user.ShopId = ConvertFromDBVal<int?>(reader.GetValue("shop_id"));
                        user.Password = reader.GetString("password");
                        user.FirstName = reader.GetString("first_name");
                        user.LastName = reader.GetString("last_name");
                        user.PhoneNumber = reader.GetString("phone_number");
                        user.Email = reader.GetString("email");
                        user.UserType = reader.GetString("user_type");
                    }
                }

                connection.Close();

                if (user != null)
                {
                    MyToken token = MyTokenHandler.CreateToken(configuration);
                    string userString = JsonConvert.SerializeObject(user);
                    string tokenString = JsonConvert.SerializeObject(token);
                    HttpContext.Session.SetString("User", userString);
                    HttpContext.Session.SetString("Token", tokenString);
                    return Ok(new { token = token, user = user });
                }

                return BadRequest();
            }

            string HashPassword(string password)
            {
                var sha = SHA512.Create();
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(sha.ComputeHash(passwordBytes));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }

        [HttpGet("usercheck")]
        public async Task<IActionResult> CheckUser()
        {
            var userString = HttpContext.Session.GetString("User");
            var tokenString = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(userString) && string.IsNullOrEmpty(tokenString))
            {
                return BadRequest("Session datalarında eksik var");
            }

            return Ok(new
            {
                user = JsonConvert.DeserializeObject<User>(userString),
                token = JsonConvert.DeserializeObject<MyToken>(tokenString)
            });
        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }
    }
}
