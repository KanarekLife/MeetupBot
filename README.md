# MeetupBot

Discord bot for keeping track of local [meetup.com](https://meetup.com/) meetups.

## How to run?

1. Clone this repository: `git clone https://github.com/KanarekLife/MeetupBot.git`
2. Enter the repository via terminal: `cd MeetupBot/`
3. Adjust configuration by editing file `docker-compose.yml`:

```yaml
ConnectionStrings__Database: "Data Source=/db/meetupbot.db"
MeetupGroupUrls__0: "https://www.meetup.com/gdansk-kubernetes-meetup-group"
MeetupGroupUrls__1: "https://www.meetup.com/sysopsgd"
MeetupGroupUrls__2: "https://www.meetup.com/rust-gdansk"
MeetupGroupUrls__3: "https://www.meetup.com/gdansk-open-source-meetup"
MeetupGroupUrls__4: "https://www.meetup.com/gdansk-typescript"
MeetupGroupUrls__5: "https://www.meetup.com/gdansk-observability-tech-community-meetup-group"
MeetupGroupUrls__6: "https://www.meetup.com/golang-user-group-trojmiasto"
MeetupGroupUrls__7: "https://www.meetup.com/infoshare"
MeetupGroupUrls__8: "https://www.meetup.com/microsoft-azure-users-group-poland"
DiscordChannels__0: "1305543064529404038"
DiscordToken: # REMEMBER TO FILL THIS UP!
MeetupAPITTLInSeconds: "43200"
```

4. Run the bot: `docker compose up -d`
