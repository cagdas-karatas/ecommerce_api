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
    public class ShopController : ControllerBase
    {
        private readonly string connectionString;

        public ShopController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("mysql_connection_string");
        }

        [HttpGet("requests")]
        public async Task<IEnumerable<ShopRequest>> GetShopRequests()
        {
            List<ShopRequest> shops = new List<ShopRequest>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("GetShopRequests", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while(reader.Read())
                    {
                        shops.Add(new ShopRequest
                        {
                            ShopRequestId = reader.GetInt32("shop_request_id"),
                            ShopName = reader.GetString("shop_name"),
                            TaxNumber = reader.GetString("tax_number"),
                            CompanyPhoneNumber = reader.GetString("company_phone_number"),
                            ShopAddress = reader.GetString("shop_address"),
                            ApproveStatus = reader.GetString("approve_status"),
                            UserId = reader.GetInt32("user_id")
                        });
                    }
                }
                connection.Close();
            }

            return shops;
        }

        [HttpPost("send_request")]
        public async Task<IActionResult> SendShopRequest(ShopRequest shopRequest)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("SendShopRequest", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("r_shop_name", shopRequest.ShopName);
                command.Parameters.AddWithValue("r_tax_number", shopRequest.TaxNumber);
                command.Parameters.AddWithValue("r_company_phone_number", shopRequest.CompanyPhoneNumber);
                command.Parameters.AddWithValue("r_shop_address", shopRequest.ShopAddress);
                command.Parameters.AddWithValue("r_user_id", shopRequest.UserId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return Ok();
            }
        }

        [Authorize]
        [HttpPost("{id}/{response}")]
        public async Task<IActionResult> AnswerShopRequest(int id, string response)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("AnswerShopRequest", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("s_request_id", id);
                command.Parameters.AddWithValue("response", response);

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();

                return Ok();
            }
        }
    }
}
