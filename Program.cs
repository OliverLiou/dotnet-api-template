using Microsoft.EntityFrameworkCore;
using TemplateApi.Models;
using TemplateApi.Services;

var builder = WebApplication.CreateBuilder(args);

var templateContext = "_TemplateContext";
var connectionStr = builder.Configuration.GetConnectionString(templateContext);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TemplateContext>(opt => opt.UseSqlServer(connectionStr));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddScoped(typeof(IRepositoryService<>), typeof(RepositoryService<>));
builder.Services.AddScoped<IRepositoryService, RepositoryService>();

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
