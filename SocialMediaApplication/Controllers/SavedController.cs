using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System.Linq;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavedController : ControllerBase
    {
        private readonly DataContext _context;

        public SavedController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SavePost(SavedPost saved)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SavedPost.Add(saved);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveSavedPost(int id)
        {
            var savedPost = _context.SavedPost.Find(id);

            if (savedPost == null)
            {
                return NotFound();
            }

            _context.SavedPost.Remove(savedPost);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("{userId}")]
        public IActionResult GetSavedPosts(int userId)
        {
            var savedPosts = _context.SavedPost
                .Where(s => s.UserId == userId)
                .ToList();

            return Ok(savedPosts);
        }
    }
}
