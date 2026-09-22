using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MultimediaEditorAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
