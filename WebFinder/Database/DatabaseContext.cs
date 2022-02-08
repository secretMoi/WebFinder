using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebFinder.Database.Tables;
using WebFinder.Models;

namespace WebFinder.Database;

public class DatabaseContext : DbContext
{
	
	
	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
	{
			
	}
		
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
		
	public DbSet<ProductTable> Product { get; set; }
	public DbSet<MonitorTable> Monitor { get; set; }
}
