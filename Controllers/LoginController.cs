using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using staj_ecommerce_api.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string connectionString;
        public LoginController(IConfiguration _configuration)
        {
            this.connectionString = _configuration.GetConnectionString("mysql_connection_string");
        }

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
                        user = new User
                        {
                            UserId = reader.GetInt32("id"),
                            UserName = reader.GetString("user_name"),
                            Password = reader.GetString("password"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            PhoneNumber = reader.GetString("phone_number"),
                            Email = reader.GetString("email"),
                            UserType = reader.GetString("user_type"),
                            ShopId = reader.GetInt32("shop_id")
                        };
                    }
                }

                connection.Close();

                if (user != null)
                {
                    string userString = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("User", userString);
                    return Ok(new { user = user });
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

            if (string.IsNullOrEmpty(userString))
            {
                return BadRequest("Session datalarında eksik var");
            }

            return Ok(new
            {
                user = JsonConvert.DeserializeObject<User>(userString)
            });
        }
    }
}
