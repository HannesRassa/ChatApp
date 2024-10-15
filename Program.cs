using BackEnd.Data;
using BackEnd.Data.Repos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("chatDb"))
    .AddScoped<GameRepo>()
    .AddScoped<GameRoomRepo>()
    .AddScoped<PlayerRepo>()
    .AddScoped<QuestionsRepo>();


var app = builder.Build();
using (var scope = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<DataContext>())
    {
        context?.Database.EnsureCreated();
    }
}

if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
