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

namespace AiCoreApi.SemanticKernel.Agents
{
    public class CsharpCodeAgent : BaseAgent, ICsharpCodeAgent
    {
        private static ConcurrentDictionary<string, List<string>> _assemblyPaths = new();
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
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute C# Code", csharpCode);

            var globals = new Globals
            {
                Parameters = parameters,
                RequestAccessor = _requestAccessor,
                ResponseAccessor = _responseAccessor,
                ExecuteAgent = ExecuteAgent
            };

            // Extract and resolve NuGet packages
            var assemblyPathsCacheKey = csharpCode.GetHash();
            _assemblyPaths.TryGetValue(assemblyPathsCacheKey, out var assemblyPaths);
            if(assemblyPaths == null)
            {
                var nugetDirectives = ExtractNuGetDirectives(csharpCode);
                assemblyPaths = await ResolveNuGetPackages(nugetDirectives);
                _assemblyPaths.TryAdd(assemblyPathsCacheKey, assemblyPaths);
            }

            var scriptOptions = ScriptOptions.Default
                .WithReferences(assemblyPaths.Select(LoadAssembly));

            // Clean the script code by removing #r directives
            var cleanedCode = Regex.Replace(csharpCode, @"#r\s+""nuget:[^""]+""", "");

            // Evaluate the script
            _plannerHelpers.CsharpCodeAgent = this;
            try
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Execution", "");
                var result = await CSharpScript.EvaluateAsync<string>(cleanedCode, scriptOptions, globals: globals);
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "C# Code Result", result);
                return result;
            }
            catch (Exception e)
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, 
                    "C# Code Error", $"Exception: {e.Message}\r\n\r\nInner Exception: {e.InnerException?.Message}");
                throw;
            }
        }


        static Assembly LoadAssembly(string path)
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
            if(!File.Exists(packagePath))
                await resource.CopyNupkgToStreamAsync(packageName, selectedVersion, new FileStream(packagePath, FileMode.Create), cache, logger, CancellationToken.None);

            var packageReader = new PackageArchiveReader(packagePath);

            // Extract managed DLLs
            var libItems = await packageReader.GetLibItemsAsync(CancellationToken.None);
            foreach (var item in libItems)
            {
                // Check framework compatibility
                if (IsCompatibleFramework(item.TargetFramework, currentFramework))
                {
                    foreach (var lib in item.Items.Where(i => i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
                    {
                        var libPath = ExtractFile(packageReader, lib, tempPath);
                        packagePaths.Add(libPath);
                    }
                }
            }

            // Extract native binaries from runtimes folder
            var runtimeItems = await packageReader.GetItemsAsync("runtimes", CancellationToken.None);
            foreach (var runtimeItem in runtimeItems)
            {
                if (!runtimeItem.TargetFramework.Equals(currentFramework) && !runtimeItem.TargetFramework.IsAny)
                    continue;
                foreach (var runtimeFile in runtimeItem.Items.Where(
                    i => i.Contains("native/") 
                        &&  i.Contains($"{runtimeIdentifier}/") 
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

                    // Recursively resolve the dependency package
                    await ResolvePackageAndDependencies(depPackageName, depVersion.ToNormalizedString(), repository, cache, logger, packagePaths, processedPackages);
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
    }

    public interface ICsharpCodeAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
