using DSharpPlus;
using DSharpPlus.Entities;
using MeetupBot.Data;
using MeetupBot.Helpers;
using MeetupBot.Services.Abstract;
using Microsoft.Extensions.Options;

namespace MeetupBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptionsMonitor<AppConfiguration> _configuration;
    private readonly IMeetupService _meetupService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly DiscordClient _client;

    public Worker(ILogger<Worker> logger, IOptionsMonitor<AppConfiguration> configuration, IMeetupService meetupService, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _meetupService = meetupService;
        _serviceScopeFactory = serviceScopeFactory;
        
        if (_configuration.CurrentValue.DiscordToken is null)
        {
            throw new Exception("DiscordToken is not set.");
        }
        
        _client = DiscordClientBuilder.CreateDefault(configuration.CurrentValue.DiscordToken!, DiscordIntents.AllUnprivileged)
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.ConnectAsync(new DiscordActivity("\ud83d\udc40 looking for new meetups", DiscordActivityType.Custom));
        _logger.LogInformation("Connected to discord");

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync(stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var groups = (await _meetupService.GetMeetupGroupsFromConfiguration(stoppingToken)).ToArray();
            var events = groups.SelectMany(x => x.Events);

            var newEvents = events
                .Where(x => !context.MeetupEvents.Any(y => y.Url == x.Url && y.Published == x.Published))
                .Select(x => (groups.First(y => y.Events.Any(z => z.Url == x.Url)), x))
                .ToArray();
            _logger.LogInformation("Found {count} new events", newEvents.Length);

            if (newEvents.Length != 0)
            {
                var channels = await Task.WhenAll(_configuration.CurrentValue.DiscordChannels.Select(id => _client.GetChannelAsync(id)));
                foreach (var (group, newEvent) in newEvents)
                {
                    foreach (var channel in channels)
                    {
                        var message = await new DiscordMessageBuilder()
                            .AddEmbed(DiscordHelpers.GetEmbedFromEvent(group, newEvent))
                            .SendAsync(channel);
                        await message.CreateReactionAsync(DiscordEmoji.FromUnicode("\ud83d\udc4d"));
                        await message.CreateReactionAsync(DiscordEmoji.FromUnicode("\ud83d\udc4e"));
                    }   
                }
                
                context.MeetupGroups.AddRange(groups);
                await context.SaveChangesAsync(stoppingToken);   
            }
            
            await Task.Delay(TimeSpan.FromSeconds(_configuration.CurrentValue.MeetupAPITTLInSeconds), stoppingToken);
        }
    }
}
