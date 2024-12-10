using AiCoreApi.Authorization;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Services.ControllersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/rbac/roles")]
    public class RbacRoleSyncController : ControllerBase
    {
        private readonly IRbacRoleSyncService _rbacRoleSyncService;
        public RbacRoleSyncController(IRbacRoleSyncService rbacRoleSyncService)           
        {
            _rbacRoleSyncService = rbacRoleSyncService;
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> List()
        {
            return Ok(await _rbacRoleSyncService.ListRbacRoleSyncs());
        }

        [HttpDelete("{rbacRoleSyncId}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteRbacRoleSync(int rbacRoleSyncId)
        {
            await _rbacRoleSyncService.DeleteRbacRoleSync(rbacRoleSyncId);
            return Ok();
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> Add([FromBody] RbacRoleSyncViewModel rbacRoleSyncViewModel)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null)
                return Unauthorized();
            rbacRoleSyncViewModel.CreatedBy = currentUser;
            await _rbacRoleSyncService.AddRbacRoleSync(rbacRoleSyncViewModel);
            return Ok(true);
        }

        [HttpPut]
        [AdminAuthorize]
        public async Task<IActionResult> Update([FromBody] RbacRoleSyncViewModel rbacRoleSyncViewModel)
        {
            var currentUser = this.GetLogin();
            if (currentUser == null)
                return Unauthorized();
            rbacRoleSyncViewModel.UpdatedBy = currentUser;
            await _rbacRoleSyncService.UpdateRbacRoleSync(rbacRoleSyncViewModel);
            return Ok(true);
        }
    }
}
