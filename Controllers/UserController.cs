using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using staj_ecommerce_api.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string connectionString;

        public UserController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = new List<User>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetUsers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32("user_id"),
                            UserName = reader.GetString("user_name"),
                            Password = reader.GetString("password"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            PhoneNumber = reader.GetString("phone_number"),
                            Email = reader.GetString("email"),
                            UserType = reader.GetString("user_type"),
                            ShopId = reader.GetInt32("shop_id")
                        });
                    }
                }
                connection.Close();
            }
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            User user = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("u_ID", id);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            UserId = reader.GetInt32("user_id"),
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
            }

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            if(ModelState.IsValid)
            {
                int result;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand("InsertUser", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("u_name", user.UserName);
                    command.Parameters.AddWithValue("u_password", HashPassword(user.Password));
                    command.Parameters.AddWithValue("u_first_name", user.FirstName);
                    command.Parameters.AddWithValue("u_last_name", user.LastName);
                    command.Parameters.AddWithValue("u_phone_number", user.PhoneNumber);
                    command.Parameters.AddWithValue("u_email", user.Email);
                    command.Parameters.AddWithValue("u_type", user.UserType);
                    command.Parameters.AddWithValue("u_shop_id", user.ShopId);
                    command.Parameters.Add("result", MySqlDbType.Int32).Direction = ParameterDirection.Output;

                    connection.Open();
                    await command.ExecuteNonQueryAsync();

                    result = (int)command.Parameters["result"].Value;
                    connection.Close();

                    if (result == 0)
                    {
                        return Conflict(new { message = "A user with the same name already exists." });
                    }

                    return Ok();
                }
            }

            return BadRequest();
            
        }

        // PUT: api/User/{id}
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            if(user.UserId != null && ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand("UpdateUser", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("u_id", user.UserId);
                    command.Parameters.AddWithValue("u_name", user.UserName);
                    command.Parameters.AddWithValue("u_password", user.Password);
                    command.Parameters.AddWithValue("u_first_name", user.FirstName);
                    command.Parameters.AddWithValue("u_last_name", user.LastName);
                    command.Parameters.AddWithValue("u_phone_number", user.PhoneNumber);
                    command.Parameters.AddWithValue("u_email", user.Email);
                    command.Parameters.AddWithValue("u_type", user.UserType);
                    command.Parameters.AddWithValue("u_shop_id", user.ShopId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();
                }
                return Ok();
            }
            

            return BadRequest();
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("DeleteUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("u_id", id);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }

            return Ok();
        }

        string HashPassword(string password)
        {
            var sha = SHA512.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(passwordBytes));
        }
    }
}
