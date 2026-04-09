using Microsoft.AspNetCore.Authentication;

namespace RVM.McpForge.API.Auth;

public class ApiKeyAuthOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKey";
    public List<ApiKeyEntry> Keys { get; set; } = [];
}

public class ApiKeyEntry
{
    public string Key { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
}
