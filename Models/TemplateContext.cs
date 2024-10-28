using Microsoft.EntityFrameworkCore;
namespace TemplateApi.Models;

public class TemplateContext(DbContextOptions<TemplateContext> options) : DbContext(options)
{
    public DbSet<Table1> Table1 { get; set; } = null!;

    public DbSet<Table1Log> Table1Log { get; set; } = null!;
}