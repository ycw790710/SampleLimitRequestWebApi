
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SampleLimitRequestWebApi.RequestRateLimits;
using System.Text;

namespace SampleLimitRequestWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = "SampleLimitRequest",
                   ValidAudience = "SampleLimitRequest",
                   IssuerSigningKeyResolver = (string unvalidToken, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                   {
                       return new[] { new SymmetricSecurityKey(GetSecretKey()) };
                   },
                   ClockSkew = TimeSpan.Zero
               };
           });
            builder.Services.AddAuthorization();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IRequestRateLimitStatusCacheService, RequestRateLimitStatusCacheService>();
            builder.Services.AddSingleton<IRequestRateLimitCacheService, RequestRateLimitCacheService>();
            builder.Services.AddSingleton<IRequestRateLimitStatusService, RequestRateLimitStatusService>();
            builder.Services.AddScoped<IRequestRateLimitService, RequestRateLimitService>();
            builder.Services.AddHostedService<RequestRateLimitStatusCacheBackgroundService>();

            var app = builder.Build();

            app.UseMiddleware<RequestRateLimitMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UserRequestRateLimitMiddleware>();// after UseAuthentication

            app.MapControllers();

            app.Run();
        }

        static byte[] GetSecretKey()
        {
            var bytes = Encoding.UTF8.GetBytes("a98dghmnibqutldimpga08hpm3h;ovihdg;029;vty;d0aest0oiassad9pnyvg39wyh08tyvaote");
            Array.Resize(ref bytes, 64);
            return bytes;
        }
    }
}