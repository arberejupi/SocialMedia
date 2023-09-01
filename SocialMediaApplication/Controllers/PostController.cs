using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly DataContext _context;

        public PostController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            var posts = _context.Post.ToList();
            return Ok(posts);
        }
        [HttpGet("myposts")]
        [Authorize]
        public IActionResult GetMyPosts()
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);

            var posts = _context.Post.Where(p => p.UserId == userId).ToList();
            return Ok(posts);
        }
        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            var post = _context.Post.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult CreatePost(Post post)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);

            post.UserId = userId;
            post.CreatedAt = DateTime.UtcNow;

            _context.Post.Add(post);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, Post updatedPost)
        {
            var post = _context.Post.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            post.Content = updatedPost.Content;

            _context.SaveChanges();

            return Ok(post);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _context.Post.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            _context.Post.Remove(post);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
