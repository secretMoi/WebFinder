using System.Net;
using Discord;
using Discord.WebSocket;
using WebFinder.Controllers;

namespace WebFinder.Services;

public class HttpRequestService : BackgroundService
{
    private readonly ILogger<HttpRequestService> _logger;
    private readonly IServiceScopeFactory  _serviceScopeFactory;
    private DiscordSocketClient  _client;

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
                
                _client = new DiscordSocketClient();
                _client.Log += Log;
                _client.Connected += ClientOnConnected;
                
                var token = "";
                // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
                // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
                // var token = File.ReadAllText("token.txt");
                // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Impossible d'automatiser la requête : ");
                _logger.LogError(ex.Message);
            }

            await Task.Delay(30000, cancellationToken);
        }
    }

    private async Task ClientOnConnected()
    {
        var user = await _client.GetUserAsync(0);
        var channel = await user.CreateDMChannelAsync();
        await channel.SendMessageAsync("coucou");
    }


    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}