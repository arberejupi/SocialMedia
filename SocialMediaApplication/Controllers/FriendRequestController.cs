using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System;
using System.Linq;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/friendrequests")]
    public class FriendRequestController : ControllerBase
    {
        private readonly DataContext _context;

        public FriendRequestController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateFriendRequest(FriendRequest friendRequest)
        {
            var existingRequest = _context.FriendRequest
                .FirstOrDefault(r => r.UserId == friendRequest.UserId && r.RequesterId == friendRequest.RequesterId);

            if (existingRequest != null)
                return BadRequest("Friend request already exists.");

            friendRequest.Status = "Pending";
            friendRequest.CreatedAt = DateTime.UtcNow;

            _context.FriendRequest.Add(friendRequest);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetFriendRequestById), new { id = friendRequest.Id }, friendRequest);
        }

        [HttpGet("{id}")]
        public IActionResult GetFriendRequestById(int id)
        {
            var friendRequest = _context.FriendRequest.FirstOrDefault(r => r.Id == id);

            if (friendRequest == null)
                return NotFound();

            return Ok(friendRequest);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetFriendRequestsByUserId(int userId)
        {
            var friendRequests = _context.FriendRequest.Where(r => r.UserId == userId).ToList();

            if (friendRequests.Count == 0)
                return NotFound();

            return Ok(friendRequests);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateFriendRequest(int id, FriendRequest updatedRequest)
        {
            var friendRequest = _context.FriendRequest.FirstOrDefault(r => r.Id == id);

            if (friendRequest == null)
                return NotFound();

            friendRequest.Status = updatedRequest.Status;
            friendRequest.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFriendRequest(int id)
        {
            var friendRequest = _context.FriendRequest.FirstOrDefault(r => r.Id == id);

            if (friendRequest == null)
                return NotFound();

            _context.FriendRequest.Remove(friendRequest);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
