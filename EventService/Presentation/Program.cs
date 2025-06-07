using Data.Contexts;
using Data.Handlers;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This line tells the serializer to handle circular references
        // by using $id and $ref properties in the JSON.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

        // Optional: Increase the maximum depth if your object graph is very deep.
        // Default is 64, which is often sufficient with ReferenceHandler.Preserve.
        options.JsonSerializerOptions.MaxDepth = 256;
    });
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
var containerName = "images";

builder.Services.AddScoped<IFileHandler>(_ => new AzureFileHandler(connectionString!, containerName));
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Retry up to 5 times
                maxRetryDelay: TimeSpan.FromSeconds(30), // Max 30 seconds delay between retries
                errorNumbersToAdd: null); // Use default transient error numbers
        });
});

builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();
//app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Service API");
    c.RoutePrefix = string.Empty;

});

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
