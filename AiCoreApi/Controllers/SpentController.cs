using AiCoreApi.Common;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/spent")]
    public class SpentController : ControllerBase
    {
        private readonly ISpentService _spentService;
        private readonly Config _config;
        public SpentController(ISpentService spentService, Config config)           
        {
            _spentService = spentService;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var spent = await _spentService.List();
            return Ok(spent);
        }

        [Authorize]
        [HttpGet("tokenCost")]
        public async Task<IActionResult> ListTokenCosts()
        {
            var spent = await _spentService.ListTokenCosts();
            return Ok(spent);
        }

        [Authorize]
        [HttpGet("price/aks")]
        public async Task<IActionResult> ListPriceAks([FromQuery] string location)
        {
            var spent = await _spentService.ListAksPrices(location);
            return Ok(spent);
        }

        [Authorize]
        [HttpGet("locations")]
        public async Task<IActionResult> ListRegions()
        {
            var spent = await _spentService.ListRegions();
            return Ok(spent);
        }
    }
}
