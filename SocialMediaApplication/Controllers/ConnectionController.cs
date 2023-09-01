using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System;
using System.Linq;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/connections")]
    public class ConnectionController : ControllerBase
    {
        private readonly DataContext _context;

        public ConnectionController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateConnection(Connection connection)
        {
            var existingConnection = _context.Connection
                .FirstOrDefault(c => c.UserId == connection.UserId && c.FriendId == connection.FriendId);

            if (existingConnection != null)
                return BadRequest("Connection already exists.");

            connection.Status = "Connected";
            connection.CreatedAt = DateTime.UtcNow;

            _context.Connection.Add(connection);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetConnectionById), new { id = connection.Id }, connection);
        }

        [HttpGet("{id}")]
        public IActionResult GetConnectionById(int id)
        {
            var connection = _context.Connection.FirstOrDefault(c => c.Id == id);

            if (connection == null)
                return NotFound();

            return Ok(connection);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetConnectionsByUserId(int userId)
        {
            var connections = _context.Connection.Where(c => c.UserId == userId).ToList();

            if (connections.Count == 0)
                return NotFound();

            return Ok(connections);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteConnection(int id)
        {
            var connection = _context.Connection.FirstOrDefault(c => c.Id == id);

            if (connection == null)
                return NotFound();

            _context.Connection.Remove(connection);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
