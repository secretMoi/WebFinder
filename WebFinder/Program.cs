using Microsoft.EntityFrameworkCore;
using WebFinder.Database;
using WebFinder.Services;

var builder = WebApplication.CreateBuilder(args);
const string _connectionString = "server=192.168.0.119;port=3306;database=WebFinder;user=root;password=root";


builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseLazyLoadingProxies()
    .UseMySql(
        _connectionString,
        ServerVersion.AutoDetect(_connectionString)
    ));
builder.Services.AddScoped<ProductRepository>();

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.AddScoped<ILdlcService, LdlcService>();

builder.Services.AddSingleton<IHostedService, HttpRequestService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
