using web_client_shared;

namespace web_client.Services.PostService
{
    public interface IPostService
    {
        Task<int> Create(Post model);
        Task<IList<Post>> GetAll();
    }
}
