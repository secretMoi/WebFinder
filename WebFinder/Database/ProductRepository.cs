using Microsoft.EntityFrameworkCore;
using WebFinder.Database.Tables;

namespace WebFinder.Database;

public class ProductRepository
{
	private readonly DatabaseContext _databaseContext;
	public DatabaseContext DatabaseContext => _databaseContext;

	public ProductRepository(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}
	
	public List<ProductTable> GetAll()
	{
		return _databaseContext.Product.ToList();
	}
	
	public bool SaveChanges()
	{
		// permet d'appliquer les modifications à la db
		return _databaseContext.SaveChanges() >= 0;
	}
	
	public void Create(ProductTable productTable)
	{
		if (productTable == null)
			throw new ArgumentNullException(nameof(productTable));

		
		if(!GetAll().Select(p => p.Url).ToList().Contains(productTable.Url))
			_databaseContext.Product.Add(productTable);
	}

		
	// public async Task<List<ProductTable>> GetAllAsync()
	// {
	// 	return await _databaseContext.Product.ToListAsync(); // retourne la liste des commandes
	// }
}