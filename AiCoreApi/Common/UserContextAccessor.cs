using AiCoreApi.Authorization;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using System.Security.Claims;

namespace AiCoreApi.Common
{
    public class UserContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILoginProcessor _loginProcessor;

        public static AsyncLocal<int?> AsyncScheduledLoginId = new();
        private int? _loginId = null;

        public UserContextAccessor(IHttpContextAccessor httpContextAccessor, ILoginProcessor loginProcessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _loginProcessor = loginProcessor;
        }

        public int? LoginId
        {
            get
            {
                if (!_loginId.HasValue)
                {
                    if (_httpContextAccessor.HttpContext == null)
                        return null;
                    if (_httpContextAccessor.HttpContext.User.Identity is not ClaimsIdentity identity)
                        return null;
                    var claims = identity.Claims.ToDictionary(key => key.Type, value => value.Value);
                    var login = claims[ClaimTypes.NameIdentifier];
                    var loginType = claims.TryGetValue(IdTokenClaims.LoginType, out var loginTypeClaimValue)
                        ? Enum.Parse<LoginTypeEnum>(loginTypeClaimValue)
                        : LoginTypeEnum.Password;

                    _loginId = _loginProcessor.GetByLogin(login, loginType).Result?.LoginId;
                }
                return _loginId;
            }
        }

        public void SetLoginId(int? loginId)
        {
            _loginId = loginId;
        }
    }
}
