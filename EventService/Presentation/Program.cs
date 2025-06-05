using Data.Contexts;
using Data.Handlers;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
var containerName = "images";

builder.Services.AddScoped<IFileHandler>(_ => new AzureFileHandler(connectionString!, containerName));
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddDbContext<DataContext>( x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();
app.MapOpenApi();

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
