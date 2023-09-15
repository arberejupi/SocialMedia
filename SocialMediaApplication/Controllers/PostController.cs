using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SocialMediaApplication.Data;
using SocialMediaApplication.Models;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace SocialMediaApplication.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _imageFolderPath = "Images";
        private readonly IOptions<AppSettings> _appSettings;

        public PostController(DataContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        
        [HttpGet]
        public IActionResult GetPosts()
        {
            try
            {
                var posts = _context.Post.ToList();

                // Create a new list to store posts with full image URLs
                var postsWithImages = new List<object>();

                foreach (var post in posts)
                {
                    // Build the full image URL
                    var fullImageUrl = Path.Combine(_appSettings.Value.ImageStoragePath, post.ImageUrl);

                    // Create a new object that includes post details and the full image URL
                    var postWithImage = new
                    {
                        post.Id,
                        post.UserId,
                        post.Content,
                        post.CreatedAt,
                        ImageUrl = fullImageUrl, // Include the full image URL
                    };

                    postsWithImages.Add(postWithImage);
                }

                return Ok(postsWithImages);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
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
        public async Task<IActionResult> CreatePost(Post post)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber).Value);
            post.UserId = userId;
            // You can set imageUrl directly to the post.ImageUrl property here

            // Save the post to the database
            _context.Post.Add(post);
            _context.SaveChanges();

            // Return the response in JSON format
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post); // You can customize the response object as needed
        }


        // Existing class definitions...



        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, Post updatedPost)
        {
            var post = _context.Post.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            post.Content = updatedPost.Content;
            post.ImageUrl = updatedPost.ImageUrl; // Add this line to update the image URL

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
        [HttpPost("upload-image")]
        [Authorize]
        public IActionResult UploadImage([FromForm] IFormFile file)
        {
            try
            {
                // Check if a file was provided
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Generate a unique filename for the uploaded image (e.g., using a GUID)
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Combine the image folder path and the unique filename
                var filePath = Path.Combine(_appSettings.Value.ImageStoragePath, uniqueFileName);

                // Save the uploaded image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return the filename with format as the response (you can save this in the database)
                return Ok(uniqueFileName);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the upload process
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
       [HttpGet("get-photo")]
        public IActionResult GetPhotoByUrl(string imageUrl)
        {
            try
            {
                // Validate the URL to ensure it's within the allowed paths
                if (!imageUrl.StartsWith(_appSettings.Value.ImageStoragePath, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Invalid image URL");
                }

                // Check if the file exists
                if (!System.IO.File.Exists(imageUrl))
                {
                    return NotFound("Image not found");
                }

                // Read the image file
                var imageBytes = System.IO.File.ReadAllBytes(imageUrl);

                // Determine the content type based on the file extension
                var contentType = GetContentType(Path.GetExtension(imageUrl));

                if (contentType == null)
                {
                    return BadRequest("Unsupported image format");
                }

                // Return the image as a file response
                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        // ... (other actions in your PostController)

        // Helper method to determine content type based on file extension
        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                // Add more supported image formats here
                default:
                    return null; // Unsupported format
            }
        }
    }
}
