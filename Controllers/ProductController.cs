using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using staj_ecommerce_api.Models;
using System.Data;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string connectionString;

        public ProductController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = new List<Product>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32("id"),
                            ProductName = reader.GetString("product_name"),
                            SellerName = reader.GetString("seller_name"),
                            ProductDescription = reader.GetString("product_description"),
                            ProductCategory = reader.GetString("product_category"),
                            Price = reader.GetDouble("price"),
                            ImageUrl = reader.GetString("image_url"),
                            Rate = reader.GetFloat("rate"),
                            CountOfReviews = reader.GetInt32("count_of_reviews")
                        });
                    }
                }
            }

            return Ok(products);
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product product = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("p_id", id);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32("id"),
                            ProductName = reader.GetString("product_name"),
                            SellerName = reader.GetString("seller_name"),
                            ProductDescription = reader.GetString("product_description"),
                            ProductCategory = reader.GetString("product_category"),
                            Price = reader.GetDouble("price"),
                            ImageUrl = reader.GetString("image_url"),
                            Rate = reader.GetFloat("rate"),
                            CountOfReviews = reader.GetInt32("count_of_reviews")
                        };
                    }
                }
            }

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("InsertProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_product_name", product.ProductName);
                command.Parameters.AddWithValue("p_seller_name", product.SellerName);
                command.Parameters.AddWithValue("p_product_description", product.ProductDescription);
                command.Parameters.AddWithValue("p_product_category", product.ProductCategory);
                command.Parameters.AddWithValue("p_price", product.Price);
                command.Parameters.AddWithValue("p_image_url", product.ImageUrl);
                command.Parameters.AddWithValue("p_rate", product.Rate);
                command.Parameters.AddWithValue("p_count_of_reviews", product.CountOfReviews);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Product/{id}
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if(product.Id != null)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand("UpdateProduct", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("p_id", product.Id);
                    command.Parameters.AddWithValue("p_product_name", product.ProductName);
                    command.Parameters.AddWithValue("p_seller_name", product.SellerName);
                    command.Parameters.AddWithValue("p_product_description", product.ProductDescription);
                    command.Parameters.AddWithValue("p_product_category", product.ProductCategory);
                    command.Parameters.AddWithValue("p_price", product.Price);
                    command.Parameters.AddWithValue("p_image_url", product.ImageUrl);
                    command.Parameters.AddWithValue("p_rate", product.Rate);
                    command.Parameters.AddWithValue("p_count_of_reviews", product.CountOfReviews);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return Ok();
                }
            }

            return BadRequest();
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("DeleteProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("p_id", id);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }

            return Ok();
        }
    }
}