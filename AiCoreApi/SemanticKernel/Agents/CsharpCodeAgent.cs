using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using System.Web;
using AiCoreApi.Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NuGet.Protocol.Core.Types;
using NuGet.Configuration;
using NuGet.Common;
using NuGet.Versioning;
using System.Reflection;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using ILogger = NuGet.Common.ILogger;
using NuGet.Frameworks;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using AiCoreApi.Common.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class CsharpCodeAgent : BaseAgent, ICsharpCodeAgent
    {
        private static ConcurrentDictionary<string, List<string>> _assemblyPaths = new();
        private static ConcurrentDictionary<string, Script<string>> _compiledScripts = new();

        private const string DebugMessageSenderName = "CSharpCodeAgent";

        private static class AgentContentParameters
        {
            public const string CsharpCode = "csharpCode";
        }

        private readonly IPlannerHelpers _plannerHelpers;
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;

        public CsharpCodeAgent(
            IPlannerHelpers plannerHelpers,
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor)
        {
            _plannerHelpers = plannerHelpers;
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
        }

        public override async Task<string> DoCall(
            AgentModel agent,
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var csharpCode = ApplyParameters(agent.Content[AgentContentParameters.CsharpCode].Value, parameters);
            var quickMode = !csharpCode.Replace(" ", "").Contains("classAgent");

            return quickMode
                ? await QuickCall(parameters, csharpCode)
                : await Call(parameters, csharpCode);
        }

        private async Task<string> Call(Dictionary<string, string> parameters, string csharpCode)
        {
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute C# Code", csharpCode);
            // Clean the script code by removing #r directives
            var cleanedCode = Regex.Replace(csharpCode, @"#r\s+"".*""", "");

            // Cache compiled scripts
            var scriptCacheKey = cleanedCode.GetHash().Replace("=", "").Replace("/", "");
            var dllPath = $"/app/{scriptCacheKey}.dll";

            if (!File.Exists(dllPath) || new FileInfo(dllPath).Length == 0)
            {
                // Extract and resolve NuGet packages
                var nugetDirectives = ExtractNuGetDirectives(csharpCode);
                var assemblyPathsJson = nugetDirectives.ToJson();
                var assemblyPathsCacheKey = assemblyPathsJson?.GetHash() ?? "default";
                _assemblyPaths.TryGetValue(assemblyPathsCacheKey, out var assemblyPaths);
                if (assemblyPaths == null)
                {
                    assemblyPaths = await ResolveNuGetPackages(nugetDirectives);
                    _assemblyPaths.TryAdd(assemblyPathsCacheKey, assemblyPaths);
                }
                // Compile the script
                var compiler = new DynamicCompiler();
                compiler.CompileCodeToDll(cleanedCode, dllPath, assemblyPaths);
            }

            try
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Execution", "");
                // Execute the script
                var executor = new DynamicAssemblyExecutor();
                Func<string, List<string>?, string> executeAgent = ExecuteAgent;
                var result = executor.Execute(dllPath, "Agent", "Run", new object[]
                {
                    parameters, _requestAccessor, _responseAccessor, executeAgent
                }).ToString();
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Result", result);
                return result;
            }
            catch (Exception e)
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Error", $"Exception: {e.Message}\r\n\r\nInner Exception: {e.InnerException?.Message}");
                throw;
            }
        }

        // Quick mode for C# code execution. Fast execution, but keep in memory all loaded assemblies permanently.
        private async Task<string> QuickCall(Dictionary<string, string> parameters, string csharpCode)
        {
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute C# Code (quick)", csharpCode);
            var globals = new Globals
            {
                Parameters = parameters,
                RequestAccessor = _requestAccessor,
                ResponseAccessor = _responseAccessor,
                ExecuteAgent = ExecuteAgent
            };

            // Clean the script code by removing #r directives
            var cleanedCode = Regex.Replace(csharpCode, @"#r\s+""nuget:[^""]+""", "");

            // Cache compiled scripts
            var scriptCacheKey = cleanedCode.GetHash().Replace("=", "").Replace("/", "");
            if (!_compiledScripts.TryGetValue(scriptCacheKey, out var compiledScript))
            {
                // Extract and resolve NuGet packages
                var nugetDirectives = ExtractNuGetDirectives(csharpCode);
                var assemblyPathsJson = nugetDirectives.ToJson();
                var assemblyPathsCacheKey = assemblyPathsJson?.GetHash() ?? "default";
                _assemblyPaths.TryGetValue(assemblyPathsCacheKey, out var assemblyPaths);
                if (assemblyPaths == null)
                {
                    assemblyPaths = await ResolveNuGetPackages(nugetDirectives);
                    _assemblyPaths.TryAdd(assemblyPathsCacheKey, assemblyPaths);
                }

                // Run the script
                var scriptOptions = ScriptOptions.Default
                    .WithReferences(assemblyPaths.Select(LoadAssembly));

                compiledScript = CSharpScript.Create<string>(cleanedCode, scriptOptions, typeof(Globals));
                _compiledScripts.TryAdd(scriptCacheKey, compiledScript);
            }
            try
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Execution (quick)", "");
                var result = await compiledScript.RunAsync(globals);
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Result (quick)", result.ReturnValue);
                return result.ReturnValue;
            }
            catch (Exception e)
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Error (quick)", $"Exception: {e.Message}\r\n\r\nInner Exception: {e.InnerException?.Message}");
                throw;
            }
        }

        private static Assembly LoadAssembly(string path)
        {
            var assemblyName = AssemblyName.GetAssemblyName(path);
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName.FullName);
            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }
            return Assembly.LoadFile(path);
        }

        private string ExecuteAgent(string agentName, List<string>? parameters = null)
        {
            _plannerHelpers.CsharpCodeAgent = this;
            try
            {
                return _plannerHelpers.ExecuteAgent(agentName, parameters).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName,
                    "C# Code ExecuteAgent Error", $"Agent: {agentName}\r\n\r\n Exception: {e.Message}\r\n\r\nInner Exception: {e.InnerException?.Message}");
                throw;
            }
        }

        // Extracts #r "nuget:" directives from the script
        private List<(string packageName, string version)> ExtractNuGetDirectives(string code)
        {
            var regex = new Regex(@"#r\s+""nuget:\s*(?<package>[^,]+),\s*(?<version>[^""]+)""");
            var matches = regex.Matches(code);

            var packages = new List<(string, string)>();
            foreach (Match match in matches)
            {
                var packageName = match.Groups["package"].Value.Trim();
                var packageVersion = match.Groups["version"].Value.Trim();
                packages.Add((packageName, packageVersion));
            }
            return packages;
        }

        // Resolves NuGet packages including dependencies
        private async Task<List<string>> ResolveNuGetPackages(List<(string packageName, string version)> packages)
        {
            var packagePaths = new List<string>();
            var cache = new SourceCacheContext();
            var logger = NullLogger.Instance;
            var providers = Repository.Provider.GetCoreV3();
            var repository = new SourceRepository(new PackageSource("https://api.nuget.org/v3/index.json"), providers);

            // To track already processed packages and avoid infinite loops
            var processedPackages = new HashSet<string>();
            foreach (var (packageName, version) in packages)
            {
                await ResolvePackageAndDependencies(packageName, version, repository, cache, logger, packagePaths, processedPackages);
            }
            return packagePaths;
        }

        // Resolves a package and its dependencies
        private async Task ResolvePackageAndDependencies(
            string packageName,
            string version,
            SourceRepository repository,
            SourceCacheContext cache,
            ILogger logger,
            List<string> packagePaths,
            HashSet<string> processedPackages)
        {
            var currentFramework = NuGetFramework.ParseFolder($"net{Environment.Version.Major}.{Environment.Version.Minor}");
            var runtimeIdentifier = RuntimeInformation.RuntimeIdentifier;

            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            var versionRange = new VersionRange(NuGetVersion.Parse(version));
            var versions = await resource.GetAllVersionsAsync(packageName, cache, logger, CancellationToken.None);
            var selectedVersion = versions.FindBestMatch(versionRange, v => v);

            if (selectedVersion == null)
            {
                if (!versions.Any())
                    throw new Exception($"Package {packageName} not found in NuGet repository");

                selectedVersion = versions.Last();
                _responseAccessor.AddDebugMessage(DebugMessageSenderName,
                    "C# Code Error", $"Nuget Package '{packageName}' version '{version}' not found in Repository. Using latest: {selectedVersion}");
            }

            // Skip if already processed
            var packageKey = $"{packageName}.{selectedVersion}";
            if (processedPackages.Contains(packageKey))
                return;

            processedPackages.Add(packageKey);

            var tempPath = Path.Combine(Path.GetTempPath(), packageName, selectedVersion.ToNormalizedString());
            Directory.CreateDirectory(tempPath);

            var packagePath = Path.Combine(tempPath, $"{packageName}.nupkg");
            if (!File.Exists(packagePath))
                await resource.CopyNupkgToStreamAsync(packageName, selectedVersion, new FileStream(packagePath, FileMode.Create), cache, logger, CancellationToken.None);

            var packageReader = new PackageArchiveReader(packagePath);

            // Extract managed DLLs
            var libItems = (await packageReader.GetLibItemsAsync(CancellationToken.None)).OrderByDescending(x => x.TargetFramework.Version).ToList();
            foreach (var item in libItems)
            {
                var isLoaded = false;
                // Check framework compatibility
                if (IsCompatibleFramework(item.TargetFramework, currentFramework))
                {
                    foreach (var lib in item.Items.Where(i => i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
                    {
                        var libPath = ExtractFile(packageReader, lib, tempPath);
                        packagePaths.Add(libPath);
                        isLoaded = true;
                    }
                }
                if (isLoaded)
                    break;
            }

            // Extract native binaries from runtimes folder
            var runtimeItems = await packageReader.GetItemsAsync("runtimes", CancellationToken.None);
            foreach (var runtimeItem in runtimeItems)
            {
                if (!runtimeItem.TargetFramework.Equals(currentFramework) && !runtimeItem.TargetFramework.IsAny)
                    continue;
                foreach (var runtimeFile in runtimeItem.Items.Where(
                    i => i.Contains("native/")
                        && i.Contains($"{runtimeIdentifier}/")
                        && (i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                            i.EndsWith(".so", StringComparison.OrdinalIgnoreCase) ||
                            i.EndsWith(".dylib", StringComparison.OrdinalIgnoreCase))))
                {
                    // Extract the native binary if managed DLL has dependencies on it
                    ExtractFile(packageReader, runtimeFile, tempPath);
                }
            }

            // Resolve and process dependencies
            var dependencies = await packageReader.GetPackageDependenciesAsync(CancellationToken.None);
            foreach (var dependencyGroup in dependencies)
            {
                foreach (var dependency in dependencyGroup.Packages)
                {
                    var depPackageName = dependency.Id;
                    var depVersionRange = dependency.VersionRange;

                    // Find best matching version for dependency
                    var depVersions = await resource.GetAllVersionsAsync(depPackageName, cache, logger, CancellationToken.None);
                    var depVersion = depVersions.FindBestMatch(depVersionRange, v => v);

                    if (depVersion == null)
                    {
                        if (!depVersions.Any())
                            throw new Exception($"Dependency {depPackageName} not found in NuGet repository");

                        depVersion = depVersions.Last();
                        _responseAccessor.AddDebugMessage(DebugMessageSenderName,
                            "C# Code Error", $"Dependency Nuget Package '{depPackageName}' version not found in Repository. Using latest: {depVersion}");
                    }

                    try
                    {
                        // Recursively resolve the dependency package
                        await ResolvePackageAndDependencies(depPackageName, depVersion.ToNormalizedString(), repository,
                            cache, logger, packagePaths, processedPackages);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Package load error (depPackageName): {e.Message}{e.InnerException?.Message}");
                    }
                }
            }
        }

        private bool IsCompatibleFramework(NuGetFramework targetFramework, NuGetFramework currentFramework)
        {
            return (targetFramework.Framework == ".NETStandard" || targetFramework.Framework == ".NETCoreApp") &&
                   targetFramework.Version <= currentFramework.Version;
        }

        private string ExtractFile(PackageArchiveReader packageReader, string filePath, string tempPath)
        {
            var libPath = Path.Combine(tempPath, filePath);
            var libDir = Path.GetDirectoryName(libPath);
            if (libDir == null)
                throw new Exception($"Invalid Nuget Package path: {libPath}");

            Directory.CreateDirectory(libDir);
            if (!File.Exists(libPath))
            {
                using var stream = packageReader.GetStream(filePath);
                using var fileStream = new FileStream(libPath, FileMode.Create);
                stream.CopyTo(fileStream);
            }

            // Copy to target directory for execution
            var destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", "linux", "native", Path.GetFileName(libPath));
            if (File.Exists(destPath))
                return libPath;
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.Copy(libPath, destPath, true);
            return libPath;
        }


        public class Globals
        {
            public Dictionary<string, string> Parameters { get; set; }
            public RequestAccessor RequestAccessor { get; set; }
            public ResponseAccessor ResponseAccessor { get; set; }
            public Func<string, List<string>?, string> ExecuteAgent { get; set; }
        }

        public class DynamicCompiler
        {
            public string CompileCodeToDll(string code, string outputDllPath, IEnumerable<string> references)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(code);

                // Use a custom AssemblyLoadContext for loading assemblies
                var loadContext = new CustomAssemblyLoadContext();
                try
                {
                    var loadedReferences = new List<MetadataReference>();
                    // Include a reference to common assemblies
                    var commonReferences = new[]
                    {
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib
                        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // System.Console
                        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location) // System.Linq
                    };
                    loadedReferences.AddRange(commonReferences);
                    // Load references from the current AppDomain
                    var assembliesReferences = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                        .Select(x => x.Location)
                        .ToList();
                    // Add custom references
                    assembliesReferences.AddRange(references);
                    foreach (var reference in assembliesReferences)
                    {
                        try
                        {
                            var assembly = loadContext.LoadFromAssemblyPath(reference);
                            loadedReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                        }
                        catch (Exception ex)
                        {
                            // Ignore missing references, duplicates etc
                        }
                    }
                    var compilation = CSharpCompilation.Create(
                        Path.GetFileNameWithoutExtension(outputDllPath),
                        new[] { syntaxTree },
                        loadedReferences,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                    using var dllStream = new FileStream(outputDllPath, FileMode.Create);
                    var result = compilation.Emit(dllStream);
                    if (!result.Success)
                    {
                        var errors = string.Join("\n", result.Diagnostics
                            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                            .Select(diagnostic => diagnostic.ToString()));
                        throw new Exception($"Compilation failed:\n{errors}");
                    }

                    return outputDllPath;
                }
                finally
                {
                    loadContext.Unload();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
        }

        public class CustomAssemblyLoadContext : AssemblyLoadContext
        {
            public CustomAssemblyLoadContext() : base(isCollectible: true) { }
            protected override Assembly Load(AssemblyName assemblyName) => null; // Prevent default resolution
        }

        public class DynamicAssemblyExecutor
        {
            public object Execute(string dllPath, string typeName, string methodName, object[] parameters)
            {
                var loadContext = new CustomAssemblyLoadContext();
                try
                {
                    var assembly = loadContext.LoadFromAssemblyPath(dllPath);
                    var type = assembly.GetType(typeName);
                    if (type == null)
                        throw new Exception($"Type '{typeName}' not found in assembly.");

                    var method = type.GetMethod(methodName);
                    if (method == null)
                        throw new Exception($"Method '{methodName}' not found in type '{typeName}'.");

                    var instance = Activator.CreateInstance(type);
                    return method.Invoke(instance, parameters);
                }
                finally
                {
                    loadContext.Unload();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
        }
    }

    public interface ICsharpCodeAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
