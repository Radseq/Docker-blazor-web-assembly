using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using web_client.Services;
using web_client.Services.PostService;

namespace web_client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var serverHost = string.IsNullOrEmpty(builder.Configuration["SERVER_HOST"]) ?
              builder.HostEnvironment.BaseAddress :
              builder.Configuration["SERVER_HOST"];

            // Check if the Blazor app is running in HTTPS
            var isHttps = builder.HostEnvironment.BaseAddress.StartsWith("https", StringComparison.OrdinalIgnoreCase);

            // Set the BaseAddress of HttpClient based on the port being used
            var baseAddress = isHttps ? "https://localhost:5001" : "http://localhost:5000";

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });


            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.MetadataUrl = "http://localhost:8080/realms/Test/.well-known/openid-configuration";
                options.ProviderOptions.Authority = "http://localhost:8080/realms/Test";
                options.ProviderOptions.ClientId = "test-client";
                options.ProviderOptions.ResponseType = "id_token token";

                options.UserOptions.NameClaim = "preferred_username";
                options.UserOptions.RoleClaim = "roles";
                options.UserOptions.ScopeClaim = "scope";
            });

            builder.Services.AddTransient<IHttpService, HttpService>();
            builder.Services.AddTransient<ILocalStorageService, LocalStorageService>();
            builder.Services.AddTransient<IPostService, PostService>();

            await builder.Build().RunAsync();
        }
    }
}
