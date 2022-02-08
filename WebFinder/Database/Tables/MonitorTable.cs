using WebFinder.Database.Tables;

namespace WebFinder.Models;

public class MonitorTable
{
	public int Id { get; set; }

	public double? MaximumPrice { get; set; }
	
	public bool isMonitoringProduct { get; set; }
	
	public int ProductId { get; set; }
	public virtual ProductTable Product { get; set; }
}