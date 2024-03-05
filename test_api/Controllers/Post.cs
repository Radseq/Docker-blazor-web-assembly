using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test_api.Models;
using test_api.Services;

namespace test_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IRepoPostService _postService;

        public PostController(IRepoPostService postService)
        {
            _postService = postService;
        }

        // POST: api/Post
        [HttpPost]
        public async Task<ActionResult<Post>> AddPost(Post post)
        {
            try
            {
                var addedPost = await _postService.AddPostAsync(post);
                return Ok(addedPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding post: {ex.Message}");
            }
        }

        // GET: api/Post/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving post: {ex.Message}");
            }
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving posts: {ex.Message}");
            }
        }
    }
}
