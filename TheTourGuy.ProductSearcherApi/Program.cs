using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProductSearcherApi.Repositories;
using TheTourGuy.Models;

var builder = WebApplication.CreateBuilder(args);

//Add all profiles.
builder.Services.AddAutoMapper(typeof(Program));
//Adds controller
builder.Services.AddControllers();
builder.Services.AddTransient<ProductRepository>();
//Adds swagger and doc only in debug
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any,8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMqConfiguration>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


app.Run();

