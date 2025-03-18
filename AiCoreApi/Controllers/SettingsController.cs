using AiCoreApi.Authorization;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;
        public SettingsController(ISettingsService settingsService)           
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        [AdminAuthorize]
        [CombinedAuthorize]
        public IActionResult ListAll()
        {
            return Ok(_settingsService.ListAll());
        }

        [HttpPost]
        [AdminAuthorize]
        [CombinedAuthorize]
        public IActionResult Save([FromBody] List<SettingsViewModel> settingsViewModels)
        {
            _settingsService.SaveAll(settingsViewModels);
            return Ok();
        }

        [HttpPost("reboot")]
        [AdminAuthorize]
        [CombinedAuthorize]
        public IActionResult Reboot()
        {
            _settingsService.Reboot();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("ui")]
        public IActionResult ListForUi()
        {
            var result = _settingsService.ListForUi();
            return Ok(result);
        }
    }
}
