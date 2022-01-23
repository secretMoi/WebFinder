using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebFinder.Models;

namespace WebFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class LdlcFinderController : ControllerBase
{
    [HttpGet(Name = "GetLdlcSourceCode")]
    public List<Product> Get()
    {
        string htmlCode = GetSourceCode("https://www.ldlc.com/fr-be/recherche/rtx%203060/");

        htmlCode = RemoveBefore(htmlCode, "class=\"pdt-item\"");

        htmlCode = RemoveAfter(htmlCode, "pagination");

        string[] htmlElements = GetSplittedElements(htmlCode, "class=\"pdt-item\"");

        List<Product> products = new List<Product>();
        foreach (string element in htmlElements)
        {
            int pFrom = element.IndexOf("<h3 class =\"title-3\">") + "<h3 class =\"title-3\">".Length;
            int pTo = element.LastIndexOf("</a></h3>");

            string result = element.Substring(pFrom, pTo - pFrom);
            result = RemoveBefore(result, ".html\">");
            products.Add(new Product
            {
                Name = result
            });
        }

        return products;
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

