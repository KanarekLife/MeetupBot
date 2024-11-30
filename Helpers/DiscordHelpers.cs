using System.Globalization;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using HtmlAgilityPack;
using MeetupBot.Data;
using NodaTime;
using NodaTime.Text;

namespace MeetupBot.Helpers;

public static partial class DiscordHelpers
{
    private const string WhenField = "Kiedy";
    private const string UrlField = "Url";
    
    private static readonly LocalDateTimePattern Pattern = LocalDateTimePattern.CreateWithInvariantCulture("dddd, MMMM d 'at' h:mm tt uuuu");
    private static readonly LocalDateTimePattern PlPattern = LocalDateTimePattern.Create("dddd dd.MM.uuuu 'o' HH:mm", new CultureInfo("pl-PL"));
    
    public static DiscordEmbed GetEmbedFromEvent(MeetupGroup group, MeetupEvent @event)
    {
        var imageMatch = ImageUrlRegex().Match(@event.Description);
        var image = imageMatch.Success ? imageMatch.Groups[1].Value : null;
        var meetupDate = GetMeetupDate(@event);

        var builder = new DiscordEmbedBuilder()
            .WithTitle(@event.Title);

        if (meetupDate is not null)
        {
            builder = builder.AddField(WhenField, PlPattern.Format(meetupDate.Value));
        }

        builder = builder
            .AddField(UrlField, @event.Url)
            .WithAuthor(name: group.Title.Replace("Events -", ""), url: group.Url, iconUrl: GetGroupImage(group))
            .WithTimestamp(DateTime.Now);
        
        if (image is not null)
        {
            builder = builder.WithImageUrl(image);
        }

        return builder.Build();
    }

    public static LocalDateTime? GetMeetupDateTimeFromEmbed(DiscordEmbed embed)
    {
        var field = embed.Fields?.FirstOrDefault(x => x.Name == WhenField);
        
        if (field?.Value is null)
        {
            return null;
        }

        return PlPattern.Parse(field.Value).GetValueOrThrow();
    }

    private static LocalDateTime? GetMeetupDate(MeetupEvent @event)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(@event.Description);
        var meetupDate = doc.DocumentNode.ChildNodes
            .SkipLast(5)
            .Select(x => x.InnerText)
            .LastOrDefault();

        if (meetupDate is null)
        {
            return null;
        }

        return new[] { DateTime.Now.Year, DateTime.Now.Year + 1 }
            .Select(year => Pattern.Parse(meetupDate + " " + year))
            .Where(x => x.Success)
            .MinBy(x => Period.Between(LocalDate.FromDateTime(DateTime.UtcNow), x.Value.Date))
            ?.Value;
    }

    private static string GetGroupImage(MeetupGroup group)
    {
        return $"https://ui-avatars.com/api/?name={string.Join('+', group.Title.Replace("Events -", "").Split(' '))}&background=random";
    }

    [GeneratedRegex("<img[^>]+src=\"([^\"]+)\"")]
    private static partial Regex ImageUrlRegex();
}