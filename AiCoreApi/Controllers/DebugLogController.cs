using AiCoreApi.Authorization;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCoreApi.Controllers;

[ApiController]
[AdminAuthorize]
[Route("api/v1/logs")]
public class DebugLogController : ControllerBase
{
    private readonly IDebugLogService _debugLogService;
    public DebugLogController(IDebugLogService debugLogService)
    {
        _debugLogService = debugLogService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> List([FromBody] DebugLogFilterViewModel debugLogFilterViewModel)
    {
        var result = await _debugLogService.List(debugLogFilterViewModel);
        return Ok(result);
    }

    [HttpPost("pagesCount")]
    [Authorize]
    public async Task<IActionResult> PagesCount([FromBody] DebugLogFilterViewModel debugLogFilterViewModel)
    {
        var result = await _debugLogService.PagesCount(debugLogFilterViewModel);
        return Ok(result);
    }


}