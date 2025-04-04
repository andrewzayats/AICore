﻿using AiCoreApi.Authorization;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using AiCoreApi.Models.ViewModels;
using System.Security.Claims;

namespace AiCoreApi.Common
{
    public class RequestAccessor
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public RequestAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                return;
            _httpContextAccessor = httpContextAccessor;
            UseMarkdown = GetParameter("use_markdown") != "false";
            UseBing = GetParameter("use_bing") == "true";
            UseCachedPlan = GetParameter("use_cached_plan") != "false";
            UseDebug = GetParameter("use_debug") == "true" && IsAdmin;
            DefaultConnectionNames = GetParameter("connection_name")?.Split(',').ToList() ?? new List<string>();
            TagsString = string.IsNullOrEmpty(GetParameter("tags")) ? "0" : GetParameter("tags");
            Query = GetParameter("q") ?? "";

            if (_httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var claims = identity.Claims.ToDictionary(key => key.Type, value => value.Value);
                // LoginTypeString
                if (claims.ContainsKey(IdTokenClaims.LoginType))
                {
                    LoginTypeString = claims[IdTokenClaims.LoginType];
                }
                // Login
                if (claims.ContainsKey(ClaimTypes.NameIdentifier))
                {
                    Login = claims[ClaimTypes.NameIdentifier];
                }
                // MessageDialog
                var request = _httpContextAccessor.HttpContext.Request;
                request.EnableBuffering();
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                var body = reader.ReadToEndAsync().Result;
                request.Body.Position = 0;
                MessageDialog = body.StartsWith("{") ? body.JsonGet<MessageDialogViewModel>() : null;
            }
        }

        public void SetRequestAccessor(string serializedRequest)
        {
            var request = serializedRequest.JsonGet<RequestAccessor>(); 
            UseMarkdown = request.UseMarkdown;
            UseBing = request.UseBing;
            UseCachedPlan = request.UseCachedPlan;
            UseDebug = request.UseDebug;
            DefaultConnectionNames = request.DefaultConnectionNames;
            TagsString = request.TagsString;
            Query = request.Query;
            LoginTypeString = request.LoginTypeString;
            Login = request.Login;
            MessageDialog = request.MessageDialog;
        }

        public bool UseMarkdown { get; set; }
        public bool UseBing { get; set; }
        public bool UseCachedPlan { get; set; }
        public bool UseDebug { get; set; }
        public bool? _isAdmin { get; set; } = null;

        public bool IsAdmin
        {
            get
            {
                if (_isAdmin == null)
                {
                    _isAdmin = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role) == "Admin";
                }
                return _isAdmin.Value;

            }
            set
            {
                _isAdmin = value;
            }
        }

        public List<string> DefaultConnectionNames { get; set; }
        public string? TagsString { get; set; }
        public string Query { get; set; }
        public string? LoginTypeString { get; set; }
        public string? Login { get; set; }
        public MessageDialogViewModel? MessageDialog { get; set; }

        public List<int> Tags => (TagsString ?? "")
            .Split(',')
            .Select(item => Convert.ToInt32(item))
            .ToList();

        public LoginTypeEnum LoginType => Enum.TryParse<LoginTypeEnum>(LoginTypeString, out var loginType) ? loginType : LoginTypeEnum.Password;

        private string? GetParameter(string parameterName) => _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Query.ContainsKey(parameterName) 
            ? _httpContextAccessor.HttpContext.Request.Query[parameterName]
            : (string?) null;
    }
}
