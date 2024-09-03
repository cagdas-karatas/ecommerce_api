using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int? shop_id)
        {
            var products = new List<Product>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("s_id", shop_id);

                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        Product product= new Product
                        {
                            ProductId = reader.GetInt32("product_id"),
                            ShopId = reader.GetInt32("shop_id"),
                            ProductName = reader.GetString("product_name"),
                            ProductDescription = reader.GetString("product_description"),
                            CategoryId = reader.GetInt32("category_id"),
                            Price = reader.GetFloat("price"),
                            ImageName = reader.GetString("image_name"),
                            ImageFile = null
                        };

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
                            ProductId = reader.GetInt32("product_id"),
                            ShopId = reader.GetInt32("shop_id"),
                            ProductName = reader.GetString("product_name"),
                            ProductDescription = reader.GetString("product_description"),
                            CategoryId = reader.GetInt32("category_id"),
                            Price = reader.GetFloat("price"),
                            ImageName = reader.GetString("image_name"),
                            ImageFile = null
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ImageName = await SaveImage(product.ImageFile);
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand("InsertProduct", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("p_shop_id", product.ShopId);
                    command.Parameters.AddWithValue("p_product_name", product.ProductName);
                    command.Parameters.AddWithValue("p_product_description", product.ProductDescription);
                    command.Parameters.AddWithValue("p_category_id", product.CategoryId);
                    command.Parameters.AddWithValue("p_price", product.Price);
                    command.Parameters.AddWithValue("p_image_name", product.ImageName);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();
                }

                return Ok();
            }

            return BadRequest();
        }

        // PUT: api/Product/{id}
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutProduct([FromForm] Product product)
        {
            if(product.ProductId != null && ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand("UpdateProduct", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("p_id", product.ProductId);
                    command.Parameters.AddWithValue("p_shop_id", product.ShopId);
                    command.Parameters.AddWithValue("p_product_name", product.ProductName);
                    command.Parameters.AddWithValue("p_product_description", product.ProductDescription);
                    command.Parameters.AddWithValue("p_category_id", product.CategoryId);
                    command.Parameters.AddWithValue("p_price", product.Price);
                    command.Parameters.AddWithValue("p_image_name", product.ImageName);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();

                    return Ok();
                }
            }

            return BadRequest();
        }

        // DELETE: api/Product/{id}
        [Authorize]
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