using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using staj_ecommerce_api.Models;
using System.Data;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly string connectionString;

        public ShoppingCartController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetShoppingCart(int u_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetShoppingCart", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("u_id", u_id);
                
                var products = new List<Product>();

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = reader.GetInt32("product_id"),
                            ShopId = reader.GetInt32("shop_id"),
                            ProductName = reader.GetString("product_name"),
                            ProductDescription = reader.GetString("product_description"),
                            CategoryId = reader.GetInt32("category_id"),
                            Price = reader.GetFloat("price"),
                            ImageName = reader.GetString("image_name"),
                            ImageFile = null
                        });
                    }
                }
                connection.Close();

                return products;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToShoppingCart(int p_id, int u_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("AddToShoppingCart", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_id", p_id);
                command.Parameters.AddWithValue("u_id", u_id);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return Ok();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFromShoppingCart(int p_id, int u_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("DeleteFromShoppingCart", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_id", p_id);
                command.Parameters.AddWithValue("u_id", u_id);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return Ok();
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearShoppingCart(int u_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("ClearShoppingCart", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("u_id", u_id);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return Ok();
            }
        }
    }
}
