using System.Net;
using WebFinder.Controllers;

namespace WebFinder.Services;

public class HttpRequestService : BackgroundService
{
    private readonly ILogger<HttpRequestService> _logger;
    private readonly IServiceScopeFactory  _serviceScopeFactory;

    public HttpRequestService(ILogger<HttpRequestService> logger, IServiceScopeFactory  serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var ldlcService = scope.ServiceProvider.GetRequiredService<ILdlcService>();
            
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // foreach(var product in await ldlcService.GetProducts("rtx 3090"))
                //     _logger.LogInformation($"[{DateTime.Now}] {product}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Impossible d'automatiser la requête : ");
                _logger.LogError(ex.Message);
            }

            await Task.Delay(30000, cancellationToken);
        }
    }
}