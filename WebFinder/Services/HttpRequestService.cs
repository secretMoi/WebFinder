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
                var productsToSend = new StringBuilder();
                var products = await ldlcService.GetProducts("rtx 3060");
                products = products.Where(product => product.IsInStock)
                    .OrderBy(product => product.Price)
                    .ToList();
                
                foreach (var product in products)
                {
                    var productText = product + " : " + product.Url;
                    productsToSend.AppendLine(productText);
                    _logger.LogInformation($"[{DateTime.Now}] {productText}");
                }

                await SendDiscordMessageAsync(productsToSend.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Impossible d'automatiser la requête : ");
                _logger.LogError(ex.Message);
            }

            await Task.Delay(30000, cancellationToken);
        }
    }

    private async Task SendDiscordMessageAsync(string privateMessage)
    {
        await _discordService.ConnectAsync();

        if(privateMessage.Length > 2000)
        {
            privateMessage = privateMessage.Substring(0, 2000);
        }
        
        await _discordService.SendPrivateMessageAsync(privateMessage);
    }
}