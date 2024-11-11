namespace MeetupBot;

public class AppConfiguration
{
    public IEnumerable<string> MeetupGroupUrls { get; init; } = [];
    public IEnumerable<ulong> DiscordChannels { get; init; } = [];
    public string? DiscordToken { get; init; }
    public ulong MeetupAPITTLInSeconds { get; init; } = 43200;
}