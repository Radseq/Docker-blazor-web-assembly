using test_api.Models;

namespace test_api.Services
{
    public interface IRepoPostService
    {
        Task<List<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int id);
        Task<Post> AddPostAsync(Post post);
    }
}
