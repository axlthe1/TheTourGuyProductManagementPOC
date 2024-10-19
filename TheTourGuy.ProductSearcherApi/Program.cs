var builder = WebApplication.CreateBuilder(args);

//Add all profiles.
builder.Services.AddAutoMapper(typeof(Program));
//Adds controller
builder.Services.AddControllers();
//Adds swagger and doc only in debug
#if DEBUG
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

