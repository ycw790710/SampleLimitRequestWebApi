
using SampleLimitRequestWebApi.RequestRateLimits;

namespace SampleLimitRequestWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IRequestRateLimitCacheService, RequestRateLimitCacheService>();
            builder.Services.AddScoped<IRequestRateLimitService, RequestRateLimitService>();

            var app = builder.Build();

            app.UseMiddleware<RequestRateLimitMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseMiddleware<UserRequestRateLimitMiddleware>();// after UseAuthentication

            app.MapControllers();

            app.Run();
        }
    }
}