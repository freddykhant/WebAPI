
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        // Register a new client
        [HttpPost("register")]
        public IActionResult RegisterClient([FromBody] Client client)
        {
            if (DBManager.InsertClient(client))
            {
                return Ok(client);
            }
            return BadRequest("Error registering client.");
        }

        // Retrieve all registered clients
        [HttpGet("getAll")]
        public IActionResult GetAllClients()
        {
            List<Client> clients = DBManager.GetAllClients();
            if (clients != null)
            {
                return Ok(clients);
            }
            return NotFound("No clients found.");
        }

        // Delete a client
        [HttpPost("delete")]
        public IActionResult DeleteClient([FromBody] Client client)
        {
            if (DBManager.DeleteClient(client))
            {
                return Ok("Client deleted successfully.");
            }
            return BadRequest($"Error deleting client");
        }

        [HttpPut("updateJobs")]
        public IActionResult UpdateClientJobs([FromBody] Client client)
        {
            if (DBManager.UpdateClientJobs(client))
            {
                return Ok("Successfully updated");
            }
            return BadRequest("Error in data update");
        }

    }
}
