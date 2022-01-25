﻿using Microsoft.AspNetCore.Mvc;
using WebFinder.Services;

namespace WebFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class LdlcFinderController : ControllerBase
{
    private readonly ILogger<LdlcFinderController> _logger;
    private readonly LdlcService _ldlcService;

    public LdlcFinderController(ILogger<LdlcFinderController> logger, LdlcService ldlcService)
    {
        _logger = logger;
        _ldlcService = ldlcService;
    }

    [HttpGet(Name = "GetLdlcSourceCode")]
    public IActionResult Get(string name)
    {
        try
        {
            var products = _ldlcService.GetProducts(name);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }
}

