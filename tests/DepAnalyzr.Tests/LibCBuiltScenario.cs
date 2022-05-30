using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DepAnalyzr.Domain.Services;
using Mono.Cecil;
using Xunit;
using static DepAnalyzr.Tests.Shared.ShellHelper;

namespace DepAnalyzr.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class LibCBuiltScenario : IAsyncLifetime
{
    public IReadOnlyList<AssemblyDefinition> AssemblyDefinitions { get; private set; } = null!;
    public AssemblyDefinition AssemblyADefinition { get; private set; } = null!;
    public AssemblyDefinition AssemblyBDefinition { get; private set; } = null!;
    public AssemblyDefinition AssemblyCDefinition { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var dotNetCliPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => Path.Combine(
                Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))!, 
                "Program Files\\dotnet\\dotnet.exe"),
            
            PlatformID.Unix => "/usr/bin/dotnet",
            
            _ => throw new NotImplementedException()
        };

        var libCDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../", "DepAnalyzr.Tests.LibC");
        var targetLibrariesBuildOutputPath = Environment.CurrentDirectory;
        
        var dotnetBuildArguments = $"build -o \"{targetLibrariesBuildOutputPath}\"";
        var dotnetBuildLibCExitCode = await ExecuteWithinDirectory(
            libCDirectory, async () => await ExecuteProcess(dotNetCliPath, dotnetBuildArguments));

        Assert.Equal(0, dotnetBuildLibCExitCode);

        var assemblyPaths = new[] {'A', 'B', 'C'}
            .Select(x => $"DepAnalyzr.Tests.Lib{x}.dll")
            .Select(x => Path.Combine(targetLibrariesBuildOutputPath, x))
            .ToList();

        Assert.All(assemblyPaths, path => Assert.True(File.Exists(path)));

        AssemblyDefinitions = MetadataLoader.LoadAssemblyDefinitions(assemblyPaths).ToList();
        var index = -1;
        
        AssemblyADefinition = AssemblyDefinitions[++index];
        AssemblyBDefinition = AssemblyDefinitions[++index];
        AssemblyCDefinition = AssemblyDefinitions[++index];
    }

    public Task DisposeAsync()
    {
        foreach (var assemblyDef in AssemblyDefinitions)
            assemblyDef.Dispose();
        
        return Task.CompletedTask;
    }
}