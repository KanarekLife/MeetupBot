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
    private static readonly LocalDateTimePattern Pattern = LocalDateTimePattern.CreateWithInvariantCulture("dddd, MMMM dd 'at' h:mm tt uuuu");
    
    public static DiscordEmbed GetEmbedFromEvent(MeetupGroup group, MeetupEvent @event)
    {
        var imageMatch = ImageUrlRegex().Match(@event.Description);
        var image = imageMatch.Success ? imageMatch.Groups[1].Value : null;
        var meetupDate = GetMeetupDate(@event);

        var builder = new DiscordEmbedBuilder()
            .WithTitle(@event.Title);

        if (meetupDate is not null)
        {
            builder = builder.AddField("Kiedy", meetupDate.Value.ToString("dddd dd.MM.uuuu 'o' HH:mm", new CultureInfo("pl-PL")));
        }

        builder = builder
            .AddField("URL", @event.Url)
            .WithAuthor(name: group.Title.Replace("Events -", ""), url: group.Url, iconUrl: GetGroupImage(group))
            .WithTimestamp(DateTime.Now);
        
        if (image is not null)
        {
            builder = builder.WithImageUrl(image);
        }

        return builder.Build();
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

        var result = Pattern.Parse(meetupDate + " " + DateTime.Now.Year);

        if (!result.Success)
        {
            return null;
        }

        return result.Value;
    }

    private static string GetGroupImage(MeetupGroup group)
    {
        return $"https://ui-avatars.com/api/?name={string.Join('+', group.Title.Replace("Events -", "").Split(' '))}&background=random";
    }

    [GeneratedRegex("<img[^>]+src=\"([^\"]+)\"")]
    private static partial Regex ImageUrlRegex();
}