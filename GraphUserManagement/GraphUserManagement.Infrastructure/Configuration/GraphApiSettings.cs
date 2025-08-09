namespace GraphUserManagement.Infrastructure.Configuration;

public class GraphApiSettings
{
    public const string SectionName = "GraphApi";
    
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://graph.microsoft.com";
    public string Scopes { get; set; } = "https://graph.microsoft.com/.default";
    public int TimeoutInSeconds { get; set; } = 60;
}