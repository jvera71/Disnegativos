using Microsoft.AspNetCore.Mvc;
using Dotmim.Sync.Web.Server;

namespace Disnegativos.Web.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly WebServerAgent _webServerAgent;

    public SyncController(WebServerAgent webServerAgent)
    {
        _webServerAgent = webServerAgent;
    }

    /// <summary>
    /// This is the entry point for all sync requests from the clients.
    /// Dotmim.Sync uses this endpoint to exchange data.
    /// </summary>
    [HttpPost]
    public Task Post()
    {
        return _webServerAgent.HandleRequestAsync(HttpContext);
    }
}
