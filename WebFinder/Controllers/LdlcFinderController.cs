using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class LdlcFinderController : ControllerBase
{
    [HttpGet(Name = "GetLdlcSourceCode")]
    public string Get()
    {
        string htmlCode = GetSourceCode("https://www.ldlc.com/fr-be/recherche/rtx%203060/");

        htmlCode = RemoveBefore(htmlCode, "listing-product");

        htmlCode = RemoveAfter(htmlCode, "pagination");

        return htmlCode;
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
        return htmlCode.Substring(htmlCode.IndexOf(stringToFind));
    }

    private string RemoveAfter(string htmlCode, string stringToFind)
    {
        int index = htmlCode.IndexOf(stringToFind);
        if (index >= 0)
            htmlCode = htmlCode.Substring(0, index);

        return htmlCode;
    }
}

