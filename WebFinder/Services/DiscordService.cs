using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using WebFinder.Configurations;

namespace WebFinder.Services;

public class DiscordService
{
	private readonly DiscordSocketClient _client;
	private readonly DiscordConfiguration _discordConfiguration;
	private IDMChannel _channel;
	private readonly List<Action> _sendingQueue = new();
	private bool _isOpeningChannel;

	public DiscordService(IOptions<DiscordConfiguration> discordConfig)
	{
		_client = new DiscordSocketClient();
		_discordConfiguration = discordConfig.Value;
	}

	public async Task ConnectAsync()
	{
		if(_client.ConnectionState == ConnectionState.Connected || _client.ConnectionState == ConnectionState.Connecting) return;

		_client.Log += Log;
		_client.Connected += ClientOnConnected;
                
		await _client.LoginAsync(TokenType.Bot, _discordConfiguration.Token);
		await _client.StartAsync();
	}

	private async Task SetChannelAsync()
	{
		if(_channel is not null || _isOpeningChannel) return;

		_isOpeningChannel = true;
		var user = await _client.GetUserAsync(_discordConfiguration.ToUserId); 
		_channel = await user.CreateDMChannelAsync();
		_isOpeningChannel = false;
	}

	public async Task SendPrivateMessageAsync(string message)
	{
		if (_client.ConnectionState != ConnectionState.Connected)
		{
			_sendingQueue.Add(async () => await SendPrivateMessageAsync(message));
			return;
		}
		
		await SetChannelAsync();
		await _channel.SendMessageAsync(message);
	}
	
	private async Task ClientOnConnected()
	{
		await SetChannelAsync();

		foreach (var messageToSend in _sendingQueue)
		{
			messageToSend.Invoke();
		}
		
		_sendingQueue.Clear();
	}

	private Task Log(LogMessage logMessage)
	{
		Console.WriteLine(logMessage.ToString());
		return Task.CompletedTask;
	}
}