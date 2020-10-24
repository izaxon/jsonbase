using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;

namespace jsonbase.Hubs
{
    public interface IApiHub
    {
        Task SendUpdated(string path);
    }

    public class ApiHub : Hub<IApiHub>
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var query = httpContext.Request.Query;
            if (!query.ContainsKey("path"))
            {
                // path missing
                Context.Abort();
            }

            var path = query["path"].ToString();
            if (!path.Contains("/"))
            {
                // path is to short
                Context.Abort();
            }
            
            Groups.AddToGroupAsync(Context.ConnectionId, path);
            return base.OnConnectedAsync();
        }

        public async Task SendUpdated(string path)
        {
            await Clients.All.SendUpdated(path);
        }
    }
}