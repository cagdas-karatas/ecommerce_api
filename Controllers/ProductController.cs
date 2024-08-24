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
        private readonly IWebHostEnvironment hostEnvironment;

        public ProductController(IConfiguration configuration, IWebHostEnvironment _hostEnvironment)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
            this.hostEnvironment = _hostEnvironment;
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
                        Product product= new Product();

                        product.Id = reader.GetInt32("id");
                        product.ProductName = reader.GetString("product_name");
                        product.SellerName = reader.GetString("seller_name");
                        product.ProductDescription = reader.GetString("product_description");
                        product.ProductCategory = reader.GetString("product_category");
                        product.Price = reader.GetFloat("price");
                        product.ImageUrl = reader.GetString("image_url");
                        product.ImageFile = null;
                        product.Rate = ConvertFromDBVal<float?>(reader.GetValue("rate"));
                        product.CountOfReviews = ConvertFromDBVal<int?>(reader.GetValue("count_of_reviews"));

                        products.Add(product);
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
                            Price = reader.GetFloat("price"),
                            ImageUrl = reader.GetString("image_url"),
                            ImageFile = null,
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
        public async Task<IActionResult> PostProduct([FromForm] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ImageUrl = await SaveImage(product.ImageFile);
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

            return BadRequest();
        }

        // PUT: api/Product/{id}
        [HttpPut]
        public async Task<IActionResult> PutProduct(Product product)
        {
            if(product.Id != null && ModelState.IsValid)
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

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName += Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}