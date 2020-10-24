using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace jsonbase.Hubs
{
    public interface IApiHub
    {
        Task SendUpdated(string path);
    }

    public class ApiHub : Hub<IApiHub>
    {
        public async Task SendUpdated(string path)
        {
            await Clients.All.SendUpdated(path);
        }
    }
}