using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebFinder.Models;

namespace WebFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class LdlcFinderController : ControllerBase
{
    private readonly ILogger<LdlcFinderController> _logger;
    private const string searchUrl = "https://www.ldlc.com/fr-be/recherche/";

    public LdlcFinderController(ILogger<LdlcFinderController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetLdlcSourceCode")]
    public IActionResult Get(string name)
    {
        try
        {       
            string htmlCode = GetSourceCode(searchUrl + name);

            htmlCode = RemoveBefore(htmlCode, "class=\"pdt-item\"");

            htmlCode = RemoveAfter(htmlCode, "pagination");

            string[] htmlElements = GetSplittedElements(htmlCode, "class=\"pdt-item\"");

            var products = GetProducts(htmlElements).Where(product => product.IsInStock == true); // affiche si uniquement en stock

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    private List<Product> GetProducts(string[] htmlElements)
    {
        List<Product> products = new List<Product>();
        foreach (string element in htmlElements)
        {
            products.Add(new Product
            {
                ImageUrl = GetImageUrl(element),
                Name = GetProductName(element),
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
        catch(Exception ex)
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
        int endPosition = element.LastIndexOf(end);
        return element.Substring(startPosition, endPosition - startPosition);
    }

    private string[] GetSplittedElements(string htmlCode, string splitString)
    {
        return htmlCode.Split(new[] { splitString }, StringSplitOptions.RemoveEmptyEntries);
    }

    private string GetSourceCode(string url)
    {
        string htmlCode;
        using (WebClient client = new WebClient())
        {
            htmlCode = client.DownloadString(url);
        }

        return htmlCode;
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

