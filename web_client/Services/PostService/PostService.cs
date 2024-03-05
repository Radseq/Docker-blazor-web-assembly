using web_client_shared;

namespace web_client.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IHttpService httpService;

        public PostService(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<int> Create(Post model)
        {
            return await httpService.Post<int>("/Post", model);
        }

        public async Task<IList<Post>> GetAll()
        {
            return await httpService.Get<List<Post>>($"/Post");
        }
    }
}
