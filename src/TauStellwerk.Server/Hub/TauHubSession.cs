namespace TauStellwerk.Hub;

public partial class TauHub
{
    public void RegisterUser(string username)
    {
        _sessionService.CreateSession(username, null, Context.ConnectionId);
    }

    public void SendHeartbeat()
    {
        _sessionService.TryUpdateSessionLastContact(Context.ConnectionId);
    }
}