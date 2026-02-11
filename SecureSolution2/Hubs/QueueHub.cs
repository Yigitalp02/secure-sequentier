using Microsoft.AspNetCore.SignalR;
using SecureSolution2.Models;
using SecureSolution2.Services;

namespace SecureSolution2.Hubs;

public class QueueHub : Hub
{
    private readonly QueueStore _store;
    public QueueHub(QueueStore store) => _store = store;

    public async Task Subscribe(string user)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, user);
        // Send existing jobs for initial state
        foreach (var j in _store.GetJobsForUser(user))
            await Clients.Caller.SendAsync("JobUpdated", j);
    }

    // Helper so QueueStore can push updates
    public static async Task Broadcast(IHubContext<QueueHub> hub, Job job) =>
        await hub.Clients.Group(job.User).SendAsync("JobUpdated", job);
}
