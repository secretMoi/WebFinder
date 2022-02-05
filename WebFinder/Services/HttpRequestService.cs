using System.Text;

namespace WebFinder.Services;

public class HttpRequestService : BackgroundService
{
    private readonly ILogger<HttpRequestService> _logger;
    private readonly IServiceScopeFactory  _serviceScopeFactory;
    private readonly DiscordService _discordService;

    public HttpRequestService(ILogger<HttpRequestService> logger, IServiceScopeFactory  serviceScopeFactory, DiscordService discordService)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _discordService = discordService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var ldlcService = scope.ServiceProvider.GetRequiredService<ILdlcService>();
            
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var products = new StringBuilder();
                foreach (var product in await ldlcService.GetProducts("rtx 3090"))
                {
                    var productText = product.ToString();
                    products.AppendLine(productText);
                    _logger.LogInformation($"[{DateTime.Now}] {product}");
                }

                await _discordService.ConnectAsync();
                await _discordService.SendPrivateMessageAsync(products.ToString().Substring(0, 2000));
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