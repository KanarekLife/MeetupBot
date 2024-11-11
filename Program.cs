using MeetupBot;
using MeetupBot.Data;
using MeetupBot.Services;
using MeetupBot.Services.Abstract;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOptionsWithValidateOnStart<AppConfiguration>()
    .BindConfiguration("");
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<IMeetupService, MeetupService>();

var host = builder.Build();
await host.RunAsync();
