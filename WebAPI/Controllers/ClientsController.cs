
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

        // Update a client's details
        // PUT api/clients/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateClient(int id, [FromBody] Client updatedClient)
        {
            updatedClient.Id = id; // Ensure the ID from the route is set on the client object
            if (DBManager.UpdateClient(updatedClient))
            {
                return Ok("Client updated successfully.");
            }
            return BadRequest("Error updating client.");
        }


        // Delete a client
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteClient(int id)
        {
            if (DBManager.DeleteClient(id))
            {
                return Ok("Client deleted successfully.");
            }
            return BadRequest($"Error deleting client with ID {id}.");
        }
    }
}
