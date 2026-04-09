using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace RVM.McpForge.Application.Services;

public class GitCloneService(ILogger<GitCloneService> logger)
{
    private static readonly string BaseCloneDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RVM.McpForge", "repos");

    public string Clone(string repositoryUrl)
    {
        var repoName = ExtractRepoName(repositoryUrl);
        var clonePath = Path.Combine(BaseCloneDir, $"{repoName}-{Guid.NewGuid().ToString()[..8]}");

        Directory.CreateDirectory(clonePath);
        logger.LogInformation("Cloning {Url} to {Path}", repositoryUrl, clonePath);

        Repository.Clone(repositoryUrl, clonePath, new CloneOptions
        {
            IsBare = false,
            RecurseSubmodules = false
        });

        logger.LogInformation("Clone complete: {Path}", clonePath);
        return clonePath;
    }

    public void Cleanup(string clonePath)
    {
        if (!Directory.Exists(clonePath)) return;
        try
        {
            foreach (var file in Directory.GetFiles(clonePath, "*", SearchOption.AllDirectories))
                File.SetAttributes(file, FileAttributes.Normal);
            Directory.Delete(clonePath, true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cleanup clone at {Path}", clonePath);
        }
    }

    private static string ExtractRepoName(string url)
    {
        var uri = url.TrimEnd('/');
        if (uri.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            uri = uri[..^4];
        var lastSlash = uri.LastIndexOf('/');
        return lastSlash >= 0 ? uri[(lastSlash + 1)..] : "repo";
    }
}
