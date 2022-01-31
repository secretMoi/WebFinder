namespace WebFinder.Database.Tables;

public class ProductTable
{
	public int Id { get; set; }
	public string? Url { get; set; }
	public string? Description { get; set; }
	public string? ImageUrl { get; set; }
	public string? Name { get; set; }
	public double? Price { get; set; }
	public bool? IsInStock { get; set; }
}