namespace MeetupBot.Data;

public class MeetupEvent
{
    public string Title { get; init; }
    public string Url { get; init; }
    public string Description { get; init; }
    public DateTime Published { get; init; }
}