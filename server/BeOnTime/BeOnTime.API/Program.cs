namespace BeOnTime.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", options =>
            {
                options.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:5173", "https://localhost:5173");
            });
        });
        
        var securityKey = builder.Configuration.GetSection("Secret").Value ?? throw new InvalidOperationException("SecurityKey is not configured.");
        
        builder.Services.AddAuthentication(options =>
        {
            //TODO: Implement authentication
        });
        
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("CorsPolicy");
        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }
}