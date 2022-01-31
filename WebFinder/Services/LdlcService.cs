using WebFinder.Database;
using WebFinder.Database.Tables;
using WebFinder.Models;

namespace WebFinder.Services;
public class LdlcService
{
    private readonly ILogger<LdlcService> _logger;
    private readonly ProductRepository _productRepository;
    private const string searchUrl = "https://www.ldlc.com/fr-be/recherche/";

    public LdlcService(ILogger<LdlcService> logger, ProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetProducts(string productToSearch)
    {
        string htmlCode = await GetSourceCode(searchUrl + productToSearch);

        htmlCode = RemoveBefore(htmlCode, "class=\"pdt-item\"");

        htmlCode = RemoveAfter(htmlCode, "pagination");

        string[] htmlElements = GetSplittedElements(htmlCode, "class=\"pdt-item\"");

        var products = GetProductsFromHtml(htmlElements).ToList();
        
        SaveInDatabase(products);

        return products;
    }

    private void SaveInDatabase(List<Product> products)
    {
        foreach (var product in products)
        {
            var productTable = new ProductTable
            {
                Url = product.Url,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Description = product.Description,
                IsInStock = product.IsInStock,
                Price = product.Price
            };
            
            _productRepository.Create(productTable);
            _productRepository.SaveChanges();
        }
        
        
    }
    
    private List<Product> GetProductsFromHtml(string[] htmlElements)
    {
        List<Product> products = new List<Product>();
        foreach (string element in htmlElements)
        {
            products.Add(new Product
            {
                Url = GetUrl(element),
                ImageUrl = GetImageUrl(element),
                Name = GetProductName(element),
                Description = GetDescription(element),
                Price = GetProductPrice(element),
                IsInStock = IsProductInStock(element)
            });
        }

        return products;
    }

    private bool IsProductInStock(string element)
    {
        return element.Contains("<em>stock</em>");
    }

    private string GetUrl(string element)
    {
        return "https://www.ldlc.com" + GetStringBetween(element, "<a href=\"", ".html\">") + ".html";
    }

    private string GetDescription(string element)
    {
        return GetStringBetween(element, "<p class=\"desc\">", "</p>");
    }

    private double GetProductPrice(string element)
    {
        string stringPrice;
        if (element.Contains("<div class=\"new-price\">")) // si il y a un prix promo
        {
            stringPrice = RemoveBefore(element, "<div class=\"new-price\">");
        }
        else // sinon il y a juste un prix normal
        {
            stringPrice = RemoveBefore(element, "<div class=\"price\"><div class=\"price\">");
        }

        stringPrice = RemoveAfter(stringPrice, "</sup></div>");
        stringPrice = stringPrice.Replace("€<sup>", ","); // permet de retirer le symbol "€" et le remplacer par une virgule
        stringPrice = stringPrice.Replace("&nbsp;", ""); // retire l'espace entre les centaines et les milliers

        try
        {
            double price = double.Parse(stringPrice); // transforme la chaine de caractère en nombre à virgule
            return price;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return 0.0;
        }
    }

    private string GetImageUrl(string element)
    {
        return GetStringBetween(element, "<img src=\"", "\" loading=\"lazy\"");
    }

    private string GetProductName(string element)
    {
        string result = GetStringBetween(element, "<h3 class =\"title-3\">", "</a></h3>"); // retire les caractères en trop pour le lien de l'image et le nom du produit

        return RemoveBefore(result, ".html\">");
    }

    private string GetStringBetween(string element, string start, string end)
    {
        int startPosition = element.IndexOf(start) + start.Length;
        int endPosition = element.IndexOf(end);
        return element.Substring(startPosition, endPosition - startPosition);
    }

    private string[] GetSplittedElements(string htmlCode, string splitString)
    {
        return htmlCode.Split(new[] { splitString }, StringSplitOptions.RemoveEmptyEntries);
    }

    private async Task<string> GetSourceCode(string url)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(url);
    }

    private string RemoveBefore(string htmlCode, string stringToFind)
    {
        return htmlCode.Substring(htmlCode.IndexOf(stringToFind) + stringToFind.Length);
    }

    private string RemoveAfter(string htmlCode, string stringToFind)
    {
        int index = htmlCode.IndexOf(stringToFind);
        if (index >= 0)
            htmlCode = htmlCode.Substring(0, index);

        return htmlCode;
    }
}
