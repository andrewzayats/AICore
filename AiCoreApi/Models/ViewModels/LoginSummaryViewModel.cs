﻿using AiCoreApi.Models.DbModels;
using Newtonsoft.Json;

namespace AiCoreApi.Models.ViewModels;

public class LoginSummaryViewModel
{
    public int LoginId { get; set; }
    public string? Login { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Password { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; } = nameof(RoleEnum.User);
    public bool IsEnabled { get; set; }
    public string? LoginType { get; set; } = nameof(LoginTypeEnum.Password);
    public List<TagViewModel> Tags { get; set; } = new();
    public List<GroupViewModel> Groups { get; set; } = new();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public int TokensLimit { get; set; } = 0;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? TokensSpent { get; set; } 
}
