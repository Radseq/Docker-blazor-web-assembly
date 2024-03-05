using MySqlConnector;
using test_api.Models;

namespace test_api.Services
{
    public class RepoPostService : IRepoPostService
    {
        private readonly MySqlConnection _connection;

        public RepoPostService(IConfiguration configuration, IHostEnvironment env)
        {
            string connectionString = string.Empty;

            //if (env.IsDevelopment())
            //{
                connectionString = configuration["ConnectionString"];
            //}
            //else
            //{
            //    connectionString = configuration["DockerConnectionString"];
            //}
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            await _connection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM Posts", _connection);

            var posts = new List<Post>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                posts.Add(new Post
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Content = reader.GetString(reader.GetOrdinal("Content"))
                });
            }

            await _connection.CloseAsync();
            return posts;
        }
        public async Task<Post> GetPostByIdAsync(int id)
        {
            await _connection.OpenAsync();
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Posts WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            var post = new Post
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Content = reader.GetString(reader.GetOrdinal("Content"))
            };

            await _connection.CloseAsync();
            return post;
        }

        public async Task<Post> AddPostAsync(Post post)
        {
            await _connection.OpenAsync();
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO Posts (Title, Content) VALUES (@Title, @Content)";
            command.Parameters.AddWithValue("@Title", post.Title);
            command.Parameters.AddWithValue("@Content", post.Content);

            await command.ExecuteNonQueryAsync();
            post.Id = (int)command.LastInsertedId;

            await _connection.CloseAsync();
            return post;
        }
    }
}
