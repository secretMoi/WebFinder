using Microsoft.AspNetCore.Mvc;
using WebFinder.Services;

namespace WebFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class LdlcFinderController : ControllerBase
{
    private readonly ILogger<LdlcFinderController> _logger;
    private readonly ILdlcService _ldlcService;

    public LdlcFinderController(ILogger<LdlcFinderController> logger, ILdlcService ldlcService)
    {
        _logger = logger;
        _ldlcService = ldlcService;
    }

    [HttpGet(Name = "GetLdlcSourceCode")]
    public async Task<IActionResult> Get(string name)
    {
        try
        {
            var products = await _ldlcService.GetProducts(name);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }
}

