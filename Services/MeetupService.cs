using System.Xml.Linq;
using MeetupBot.Data;
using MeetupBot.Services.Abstract;
using NodaTime;
using NodaTime.Text;

namespace MeetupBot.Services;

public class MeetupService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<MeetupService> logger) : IMeetupService
{
    private static readonly ZonedDateTimePattern MeetupDateTimePattern = ZonedDateTimePattern.CreateWithInvariantCulture("ddd, dd MMM uuuu HH:mm:ss z", DateTimeZoneProviders.Tzdb);

    public async Task<IEnumerable<MeetupGroup>> GetMeetupGroupsFromConfiguration(CancellationToken stoppingToken)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var fetchGroupTasks = configuration.GetSection("MeetupGroupUrls")
            .GetChildren()
            .Select(section => section.Value!)
            .Select(url => GetMeetupGroupByUrl(url, httpClient, stoppingToken))
            .ToArray();
        
        return (await Task.WhenAll(fetchGroupTasks)).Where(group => group is not null).Select(group => group!);
    }

    private async Task<MeetupGroup?> GetMeetupGroupByUrl(string url, HttpClient httpClient, CancellationToken stoppingToken)
    {
        var response = await httpClient.GetAsync($"{url}/events/rss", stoppingToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError($"Failed to get meetup groups from url: \"{url}/events/rss\". Reason: {await response.Content.ReadAsStringAsync(stoppingToken)}");
            return null;
        }

        var stream = await response.Content.ReadAsStreamAsync(stoppingToken);
        var channel = XElement.Load(stream).Element("channel")!;

        return new MeetupGroup
        {
            Title = channel.Element("title")!.Value,
            Url = channel.Element("link")!.Value,
            Description = channel.Element("description")!.Value,
            Published = MeetupDateTimePattern
                .Parse(channel.Element("lastBuildDate")!.Value.Replace("EDT", "America/New_York")).Value
                .ToDateTimeUtc(),
            Events = channel.Descendants("item")
                .Select(item => new MeetupEvent
                {
                    Title = item.Element("title")!.Value,
                    Url = item.Element("guid")!.Value,
                    Description = item.Element("description")!.Value,
                    Published = MeetupDateTimePattern.Parse(item.Element("pubDate")!.Value.Replace("EDT", "America/New_York")).Value
                        .ToDateTimeUtc()
                })
                .ToArray()
        };
    }
}