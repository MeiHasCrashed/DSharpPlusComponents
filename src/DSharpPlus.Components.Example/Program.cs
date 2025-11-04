using DSharpPlus;
using DSharpPlus.Components.Example;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new InvalidOperationException("DISCORD_TOKEN environment variable not set");

builder.Services
    .AddDiscordClient(token, DiscordIntents.AllUnprivileged)
    .AddHostedService<BotService>();
    
var host = builder.Build();
host.Run();