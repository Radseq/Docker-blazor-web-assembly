
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authentication.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using test_api.Services;

namespace test_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.WebHost.UseUrls("http://*:5000");
            //builder.WebHost.UseUrls("http://172.18.0.2:5000", "https://172.18.0.2:5001");
            // Add services to the container.

            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddKeycloakAuthentication(configuration, configureOptions =>
            {
                configureOptions.TokenValidationParameters.ValidIssuers = new List<string> { "http://localhost:8080/realms/Test",
                    "http://keycloak:8080/realms/Test" };
            });

            builder.Services.AddControllers();
            builder.Services.AddTransient<IRepoPostService, RepoPostService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var openIdConnectUrl = $"{configuration["test"] ?? configuration["Keycloak:auth-server-url"]}realms/{configuration["Keycloak:realm"]}/.well-known/openid-configuration";

            services.AddSwaggerGen(c =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Auth",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri(openIdConnectUrl),
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, Array.Empty<string>()}
                });
            });

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            /// {
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            // Enable CORS
            app.UseCors(x => x
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true) // allow any origin 
                  .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
