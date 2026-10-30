using DevWork.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SeedDb>();

var app = builder.Build();

await SeedDataAsync(app);

static async Task SeedDataAsync(WebApplication app)
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        SeedDb service = scope.ServiceProvider.GetRequiredService<SeedDb>();
        await service.SeedAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())

{

    app.MapOpenApi();

    app.UseSwaggerUI(options =>

    {
        options.SwaggerEndpoint("/openapi/v1.json", "DevWork.API v1");
    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());


app.Run();
