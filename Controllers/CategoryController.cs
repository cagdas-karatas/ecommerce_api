using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using staj_ecommerce_api.Models;
using System.Data;

namespace staj_ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        string connectionString;
        public CategoryController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetCategories", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();

                var categories = new List<Category>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while(reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryId = reader.GetInt32("category_id"),
                            CategoryName = reader.GetString("category_name"),
                        });
                    }
                }

                connection.Close();

                if (categories.Count > 0)
                {
                    return Ok(categories);
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
