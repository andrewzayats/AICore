using AiCoreApi.Authorization;
using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Config _config;
        public UserController(IUserService userService, Config config)           
        {
            _userService = userService;
            _config = config;
        }

        [Authorize]
        [HttpGet("{loginId}")]
        public async Task<IActionResult> GetUser(int loginId)
        {
            var currentUser = this.GetLogin(); 
            if (currentUser == null) return Unauthorized();

            var login = await _userService.GetLoginById(loginId);
            return Ok(login);
        }

        [AdminAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var logins = await _userService.ListLoginsWithSpent();
            return Ok(logins);
        }

        [AdminAuthorize]
        [HttpPost("{loginId}/enable")]
        public async Task<IActionResult> EnableUser(int loginId)
        {
            await _userService.EnabledChange(loginId, true);
            return Ok(true);
        }

        [AdminAuthorize]
        [HttpPost("{loginId}/disable")]
        public async Task<IActionResult> DisableUser(int loginId)
        {
            await _userService.EnabledChange(loginId, false);
            return Ok(true);
        }

        [AdminAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]LoginSummaryViewModel loginSummaryViewModel)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null) return Unauthorized();

            loginSummaryViewModel.CreatedBy = currentUser;

            var model = await _userService.Add(loginSummaryViewModel);
            return Ok(model != null);
        }

        [AdminAuthorize]
        [HttpPut("{loginId}")]
        public async Task<IActionResult> UpdateUser(int loginId, [FromBody] EditLoginViewModel editLoginViewModel)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null) return Unauthorized();

            var saved = await _userService.Update(loginId, editLoginViewModel);

            return Ok(saved);
        }

        [AdminAuthorize]
        [HttpDelete("{loginId}")]
        public async Task<IActionResult> DeleteUser(int loginId)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null) return Unauthorized();

            var removed = await _userService.Delete(loginId);

            return Ok(removed);
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null) return Unauthorized();

            var result = await _userService.ChangePassword(currentUser!, changePasswordViewModel);
            return Ok(result);
        }
    }
}
