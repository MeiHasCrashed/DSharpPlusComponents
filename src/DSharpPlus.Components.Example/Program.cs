using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Components.Example;
using DSharpPlus.Components.Extensions;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new InvalidOperationException("DISCORD_TOKEN environment variable not set");

builder.Logging
    .ClearProviders()
    .SetMinimumLevel(LogLevel.Debug)
    .AddSimpleConsole(c => c.SingleLine = true);

builder.Services
    .AddDiscordClient(token, DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents)
    .AddComponentsExtension()
    .AddCommandsExtension((_, commands) =>
    {
        commands.AddCommands(Assembly.GetAssembly(typeof(BotService))!);
        var textProcessor = new TextCommandProcessor(new TextCommandConfiguration
        {
            PrefixResolver = new DefaultPrefixResolver(true, "!").ResolvePrefixAsync
        });
        commands.AddProcessor(textProcessor);
    }, new CommandsConfiguration{ RegisterDefaultCommandProcessors = false})
    .AddHostedService<BotService>();
    
var host = builder.Build();
host.Run();