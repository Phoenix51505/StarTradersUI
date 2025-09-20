namespace StarTradersUI.Api.Information;

public class ServerInformation
{
    public string Status { get; set; }
    public string Version { get; set; }
    public DateTime ResetDate { get; set; }
    public string Description { get; set; }
    public ServerStatistics Stats { get; set; }
    public ServerHealth Health { get; set; }
    public ServerLeaderboards Leaderboards { get; set; }
    public ServerResets ServerResets { get; set; }
    public ServerAnnouncement[] Announcements { get; set; }
    public ServerLink[] Links { get; set; }
}