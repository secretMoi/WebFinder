using WebFinder.Models;

namespace WebFinder.Services;

public interface ILdlcService
{
	Task<List<Product>> GetProducts(string productToSearch);
}