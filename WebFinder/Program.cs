using Microsoft.EntityFrameworkCore;
using WebFinder.Database;
using WebFinder.Services;

var builder = WebApplication.CreateBuilder(args);
const string _connectionString = "server=localhost;port=3306;database=WebFinder;user=root;password=root";

builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseLazyLoadingProxies()
    .UseMySql(
        _connectionString,
        ServerVersion.AutoDetect(_connectionString)
    ));

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<LdlcService>();
builder.Services.AddTransient<IHostedService, HttpRequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder.AllowAnyOrigin());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
