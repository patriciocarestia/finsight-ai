using System.Net;
using Hangfire.Dashboard;

namespace DolarEnVivo.API.Middleware;

public class HangfireLocalFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var ip = context.GetHttpContext().Connection.RemoteIpAddress;
        return ip is not null && IPAddress.IsLoopback(ip);
    }
}
