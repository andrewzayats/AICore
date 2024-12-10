using AiCoreApi.Authorization;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiCoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/connections")]
public class ConnectionsController : ControllerBase
{
    private readonly IConnectionService _connectionService;
    public ConnectionsController(IConnectionService connectionService)           
    {
        _connectionService = connectionService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ConnectionsList()
    {
        var currentUser = this.GetLogin();
        if (currentUser == null) return Unauthorized();

        var connections = await _connectionService.ListConnections();

        // Non-admin users should not see connection content as we have credentials in it
        if (Request.HttpContext.User.FindFirstValue(ClaimTypes.Role) != "Admin")
            connections.ForEach(c => c.Content = new Dictionary<string, string>());
        return Ok(connections);
    }

    [HttpPost]
    [AdminAuthorize]
    public async Task<IActionResult> ConnectionsAdd([FromBody] ConnectionViewModel connectionViewModel)
    {
        var currentUser = this.GetLogin();
        if (currentUser == null) return Unauthorized();

        await _connectionService.AddConnection(connectionViewModel, currentUser);
        return Ok(true);
    }

    [HttpPut("{connectionId}")]
    [AdminAuthorize]
    public async Task<IActionResult> ConnectionsUpdate([FromBody] ConnectionViewModel connectionViewModel)
    {
        await _connectionService.UpdateConnection(connectionViewModel);
        return Ok(true);
    }

    [HttpDelete("{connectionId}")]
    [AdminAuthorize]
    public async Task<IActionResult> ConnectionsDelete(int connectionId)
    {
        await _connectionService.DeleteConnection(connectionId);
        return Ok(true);
    }

    [HttpGet("{connectionId}")]
    [AdminAuthorize]
    public async Task<IActionResult> GetConnection(int connectionId)
    {
        var connection = await _connectionService.GetConnectionById(connectionId);
        return Ok(connection);
    }
}
