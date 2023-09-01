using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _context;

        public CommentController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateComment(Comment comment)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);
            comment.UserId = userId;

            comment.CreatedAt = DateTime.UtcNow;

            _context.Comment.Add(comment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateComment(int id, Comment updatedComment)
        {
            var comment = _context.Comment.FirstOrDefault(c => c.Id == id);

            if (comment == null)
                return NotFound();

            // Ensure that only the authorized user can update their own comment
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);
            if (comment.UserId != userId)
                return Forbid();

            comment.Content = updatedComment.Content;

            _context.SaveChanges();

            return Ok(comment);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comment.FirstOrDefault(c => c.Id == id);

            if (comment == null)
                return NotFound();

            // Ensure that only the authorized user can delete their own comment
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);
            if (comment.UserId != userId)
                return Forbid();

            _context.Comment.Remove(comment);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetCommentById(int id)
        {
            var comment = _context.Comment.FirstOrDefault(c => c.Id == id);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        [HttpGet("post/{postId}")]
        public IActionResult GetCommentsByPostId(int postId)
        {
            var comments = _context.Comment.Where(c => c.PostId == postId).ToList();

            if (comments.Count == 0)
                return NotFound();

            return Ok(comments);
        }

        [HttpGet("post/{postId}/check")]
        [Authorize]
        public IActionResult CheckIfCommented(int postId)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);

            var comment = _context.Comment.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            return Ok(comment != null);
        }
        [HttpGet("post/{postId}/count")]
        public IActionResult GetCommentCountByPostId(int postId)
        {
            var commentCount = _context.Comment.Count(c => c.PostId == postId);

            return Ok(commentCount);
        }
    }
}
