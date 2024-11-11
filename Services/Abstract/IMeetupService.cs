using MeetupBot.Data;

namespace MeetupBot.Services.Abstract;

public interface IMeetupService
{
    Task<IEnumerable<MeetupGroup>> GetMeetupGroupsFromConfiguration(CancellationToken stoppingToken);
}