using System.Net;
using WebFinder.Controllers;

namespace WebFinder.Services
{
    public class HttpRequestService : BackgroundService
    {
        private readonly ILogger<HttpRequestService> _logger;
        private readonly LdlcService _ldlcService;

        public HttpRequestService(ILogger<HttpRequestService> logger, LdlcService ldlcService)
        {
            _logger = logger;
            _ldlcService = ldlcService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    try
            //    {
            //        foreach(var product in await _ldlcService.GetProducts("rtx 3090"))
            //            _logger.LogInformation($"[{DateTime.Now}] {product}");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError("Impossible d'automatiser la requête : ");
            //        _logger.LogError(ex.Message);
            //    }

            //    await Task.Delay(30000, cancellationToken);
            //}
        }
    }
}
