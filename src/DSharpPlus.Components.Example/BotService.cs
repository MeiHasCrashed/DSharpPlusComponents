using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.Components.Example;

public class BotService(ILogger<BotService> logger, DiscordClient client) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await client.ConnectAsync();
        logger.LogInformation("Bot is connected and running.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.DisconnectAsync();
    }
}