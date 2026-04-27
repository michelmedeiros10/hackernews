using HackerNewsService.Infra;
using HackerNewsService.Service;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICache, Cache>();

builder.Services.AddHttpClient<IStoriesService, StoriesService>(client =>
{
	client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");
	client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
