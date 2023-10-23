using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        // Endpoint to record a job's completion
        [HttpPost("complete")]
        public IActionResult CompleteJob([FromBody] Job job)
        {
            if (DBManager.InsertJob(job))
            {
                return Ok("Job completion recorded successfully.");
            }
            return BadRequest("Error recording job completion.");
        }

        // Endpoint to retrieve all jobs
        [HttpGet("all")]
        public IActionResult GetAllJobs()
        {
            List<Job> jobs = DBManager.GetAllJobs();
            if (jobs != null && jobs.Count > 0)
            {
                return Ok(jobs);
            }
            return NotFound("No jobs found.");
        }

        // Endpoint to retrieve a specific job by its ID
        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            Job job = DBManager.GetJobById(id);
            if (job != null)
            {
                return Ok(job);
            }
            return NotFound($"Job with ID {id} not found.");
        }

        // Endpoint to update a job's details
        [HttpPut("update")]
        public IActionResult UpdateJob([FromBody] Job job)
        {
            if (DBManager.UpdateJob(job))
            {
                return Ok("Job updated successfully.");
            }
            return BadRequest("Error updating job.");
        }

        // Endpoint to delete a job using its ID
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteJob(int id)
        {
            if (DBManager.DeleteJob(id))
            {
                return Ok("Job deleted successfully.");
            }
            return BadRequest($"Error deleting job with ID {id}.");
        }
    }
}
