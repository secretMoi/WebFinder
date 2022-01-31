using Microsoft.EntityFrameworkCore;
using WebFinder.Database.Tables;
using WebFinder.Models;

namespace WebFinder.Database;

public class ProductRepository
{
	private readonly DatabaseContext _databaseContext;
	public DatabaseContext DatabaseContext => _databaseContext;

	public ProductRepository(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}
		
	public async Task<List<ProductTable>> GetAllAsync()
	{
		return await _databaseContext.Product.ToListAsync(); // retourne la liste des commandes
	}
}