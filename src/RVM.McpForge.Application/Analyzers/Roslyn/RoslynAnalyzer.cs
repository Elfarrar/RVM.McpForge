using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RVM.McpForge.Domain.Entities;
using RVM.McpForge.Domain.Enums;

namespace RVM.McpForge.Application.Analyzers.Roslyn;

public class RoslynAnalyzer : ISourceAnalyzer
{
    public async Task<AnalysisSnapshot> AnalyzeAsync(ForgeProject project, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(project.RepositoryPath))
            throw new InvalidOperationException("RepositoryPath is required for Git analysis.");

        var snapshot = new AnalysisSnapshot
        {
            ForgeProjectId = project.Id,
            SourceType = SourceType.Git
        };

        var csFiles = Directory.GetFiles(project.RepositoryPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                     && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .ToList();

        snapshot.ProjectCount = Directory.GetFiles(project.RepositoryPath, "*.csproj", SearchOption.AllDirectories).Length;

        foreach (var file in csFiles)
        {
            ct.ThrowIfCancellationRequested();
            var code = await File.ReadAllTextAsync(file, ct);
            var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: ct);
            var root = await tree.GetRootAsync(ct);

            AnalyzeControllers(root, snapshot);
            AnalyzeEntities(root, snapshot);
            AnalyzeServices(root, snapshot);
        }

        snapshot.TotalEndpoints = snapshot.Endpoints.Count;
        snapshot.TotalEntities = snapshot.Entities.Count;
        snapshot.TotalServices = snapshot.Services.Count;

        return snapshot;
    }

    private static void AnalyzeControllers(SyntaxNode root, AnalysisSnapshot snapshot)
    {
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(c => c.BaseList?.Types.Any(t => t.ToString().Contains("Controller")) == true
                     || c.AttributeLists.Any(a => a.ToString().Contains("ApiController")));

        foreach (var cls in classes)
        {
            var controllerName = cls.Identifier.Text;
            var ns = GetNamespace(cls);

            var methods = cls.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword));

            foreach (var method in methods)
            {
                var httpMethod = GetHttpMethod(method);
                var route = GetRoute(method, cls);

                snapshot.Endpoints.Add(new DiscoveredEndpoint
                {
                    AnalysisSnapshotId = snapshot.Id,
                    HttpMethod = httpMethod,
                    Route = route,
                    ControllerName = controllerName,
                    ActionName = method.Identifier.Text,
                    ReturnType = method.ReturnType.ToString(),
                    RequestBodyType = GetRequestBodyType(method)
                });
            }
        }
    }

    private static void AnalyzeEntities(SyntaxNode root, AnalysisSnapshot snapshot)
    {
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(c => c.Members.OfType<PropertyDeclarationSyntax>()
                .Any(p => p.Identifier.Text == "Id"));

        foreach (var cls in classes)
        {
            if (cls.BaseList?.Types.Any(t => t.ToString().Contains("Controller")) == true)
                continue;

            var ns = GetNamespace(cls);
            var properties = cls.Members.OfType<PropertyDeclarationSyntax>()
                .Select(p => new { Name = p.Identifier.Text, Type = p.Type.ToString() })
                .ToList();

            snapshot.Entities.Add(new DiscoveredEntity
            {
                AnalysisSnapshotId = snapshot.Id,
                Name = cls.Identifier.Text,
                Namespace = ns,
                BaseType = cls.BaseList?.Types.FirstOrDefault()?.ToString(),
                PropertiesJson = System.Text.Json.JsonSerializer.Serialize(properties)
            });
        }
    }

    private static void AnalyzeServices(SyntaxNode root, AnalysisSnapshot snapshot)
    {
        var interfaces = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>()
            .Where(i => i.Identifier.Text.StartsWith('I') && i.Identifier.Text.Length > 1);

        foreach (var iface in interfaces)
        {
            var ns = GetNamespace(iface);
            var methods = iface.Members.OfType<MethodDeclarationSyntax>()
                .Select(m => new { Name = m.Identifier.Text, ReturnType = m.ReturnType.ToString() })
                .ToList();

            snapshot.Services.Add(new DiscoveredService
            {
                AnalysisSnapshotId = snapshot.Id,
                Name = iface.Identifier.Text,
                Namespace = ns,
                IsInterface = true,
                MethodsJson = System.Text.Json.JsonSerializer.Serialize(methods)
            });
        }
    }

    private static string GetNamespace(SyntaxNode node)
    {
        var ns = node.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
        if (ns is not null) return ns.Name.ToString();

        var blockNs = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        return blockNs?.Name.ToString() ?? string.Empty;
    }

    private static string GetHttpMethod(MethodDeclarationSyntax method)
    {
        var attrs = method.AttributeLists.SelectMany(a => a.Attributes).Select(a => a.Name.ToString());
        if (attrs.Any(a => a.Contains("HttpPost"))) return "POST";
        if (attrs.Any(a => a.Contains("HttpPut"))) return "PUT";
        if (attrs.Any(a => a.Contains("HttpDelete"))) return "DELETE";
        if (attrs.Any(a => a.Contains("HttpPatch"))) return "PATCH";
        return "GET";
    }

    private static string GetRoute(MethodDeclarationSyntax method, ClassDeclarationSyntax controller)
    {
        var controllerRoute = controller.AttributeLists
            .SelectMany(a => a.Attributes)
            .FirstOrDefault(a => a.Name.ToString().Contains("Route"))
            ?.ArgumentList?.Arguments.FirstOrDefault()?.ToString().Trim('"') ?? string.Empty;

        var methodRoute = method.AttributeLists
            .SelectMany(a => a.Attributes)
            .Where(a => a.Name.ToString().Contains("Http") || a.Name.ToString().Contains("Route"))
            .SelectMany(a => a.ArgumentList?.Arguments ?? Enumerable.Empty<AttributeArgumentSyntax>())
            .FirstOrDefault()?.ToString().Trim('"') ?? string.Empty;

        if (string.IsNullOrEmpty(controllerRoute)) return methodRoute;
        if (string.IsNullOrEmpty(methodRoute)) return controllerRoute;
        return $"{controllerRoute}/{methodRoute}";
    }

    private static string? GetRequestBodyType(MethodDeclarationSyntax method)
    {
        var bodyParam = method.ParameterList.Parameters
            .FirstOrDefault(p => p.AttributeLists.Any(a => a.ToString().Contains("FromBody")));
        return bodyParam?.Type?.ToString();
    }
}
